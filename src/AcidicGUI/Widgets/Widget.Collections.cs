using System.Collections;

namespace AcidicGUI.Widgets;

public partial class Widget
{
    public int ChildCount => Children.Count;
    
    public IEnumerable<Widget> EnumerateChildren()
    {
        foreach (Widget child in children)
            yield return child;
    }
    
    public bool ContainsChild(Widget? widget)
    {
        if (widget == null)
            return false;

        return widget.BelongsToParent(this);
    }

    public bool BelongsToParent(Widget widget)
    {
        Widget? p = this;

        while (p != null)
        {
            if (p == widget)
                return true;

            p = p.Parent;
        }

        return false;
    }
    
    internal IEnumerable<Widget> CollapseChildren(bool reverse, bool includeDisabled)
    {
        if (reverse)
        {
            for (var i = children.Count - 1; i >= 0; i--)
            {
                if (!children[i].enabled && !includeDisabled)
                    continue;
                
                foreach (Widget child in children[i].CollapseChildren(reverse, includeDisabled))
                    yield return child;
                
                yield return children[i];
            }
        }
        else
        {
            for (var i = 0; i < children.Count; i++)
            {
                if (!children[i].enabled && !includeDisabled)
                    continue;
                
                foreach (Widget child in children[i].CollapseChildren(reverse, includeDisabled))
                    yield return child;
                
                yield return children[i];
            }
        }
    }
    
    internal sealed class WidgetCollection : IOrderedCollection<Widget>
    {
        private readonly Widget parent;
        private readonly List<Widget> children = new();

        internal WidgetCollection(Widget parent)
        {
            this.parent = parent;
        }
        
        public IEnumerator<Widget> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Widget item)
        {
            if (item.Parent != null)
                throw new InvalidOperationException(
                    "This widget is already a member of another parent widget. Please remove it from its parent first.");

            item.Parent = this.parent;
            this.children.Add(item);

            this.parent.InvalidateLayout();
        }

        public void Clear()
        {
            foreach (Widget item in children)
            {
                item.Parent = null;
                item.InvalidateLayout();
            }

            children.Clear();
            this.parent.InvalidateLayout();
        }

        public bool Contains(Widget item)
        {
            return item.Parent == parent && children.Contains(item);
        }

        public void CopyTo(Widget[] array, int arrayIndex)
        {
            children.CopyTo(array, arrayIndex);
        }

        public bool Remove(Widget item)
        {
            if (item.Parent != this.parent)
                throw new InvalidOperationException("The specified widget is not a member of this parent.");

            bool wasSuccess = children.Remove(item);

            if (wasSuccess)
            {
                item.parent = null;
                this.parent.InvalidateLayout();
            }

            return wasSuccess;
        }

        public int Count => children.Count;
        public bool IsReadOnly => parent.guiManager?.IsRendering == true;

        public Widget this[int index] => children[index];
    }

    internal sealed class TopLevelCollection : IOrderedCollection<Widget>
    {
        private readonly GuiManager guiManager;
        private readonly List<Widget> topLevels = new();

        internal TopLevelCollection(GuiManager guiManager)
        {
            this.guiManager = guiManager;
        }


        public IEnumerator<Widget> GetEnumerator()
        {
            return topLevels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Widget item)
        {
            Associate(item);
            topLevels.Add(item);
            item.InvalidateLayout();
        }

        public void Clear()
        {
            for (var i = 0; i < Count; i++)
                Disassociate(topLevels[i]);

            topLevels.Clear();
        }

        public bool Contains(Widget item)
        {
            return item.GuiManager == guiManager && topLevels.Contains(item);
        }

        public void CopyTo(Widget[] array, int arrayIndex)
        {
            topLevels.CopyTo(array, arrayIndex);
        }

        public bool Remove(Widget item)
        {
            if (!TryDisassociate(item))
                return false;

            return topLevels.Remove(item);
        }

        private bool TryDisassociate(Widget item)
        {
            if (item.guiManager != guiManager)
                return false;

            if (item.Parent != null)
                return false;

            item.guiManager = null;
            return true;
        }

        private void Disassociate(Widget item)
        {
            if (!TryDisassociate(item))
                throw new InvalidOperationException("The given widget is not a top-level.");
        }

        private void Associate(Widget item)
        {
            if (item.Parent != null)
                throw new InvalidOperationException(
                    "The given widget is already associated with a parent, and cannot be added as a top-level. Please remove it from its parent first.");

            if (item.guiManager != null)
                throw new InvalidOperationException("The given widget is already a top-level of another GuiManager.");

            item.guiManager = this.guiManager;
        }
        
        public int Count => topLevels.Count;
        public bool IsReadOnly => guiManager.IsRendering;

        public Widget this[int index] => topLevels[index];
    }
}