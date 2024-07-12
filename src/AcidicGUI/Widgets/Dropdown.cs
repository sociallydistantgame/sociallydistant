using System.Collections;
using AcidicGUI.Events;
using AcidicGUI.Layout;
using AcidicGUI.ListAdapters;
using AcidicGUI.Rendering;
using Microsoft.Xna.Framework;

namespace AcidicGUI.Widgets;

public abstract class Dropdown<TItemType, TView> : Widget,
    IMouseClickHandler,
    IGainFocusHandler,
    ILoseFocusHandler
    where TView : DropdownItemView<TItemType>, new()
{
    private readonly Box                 dropdownBox = new();
    private readonly Box                 buttonBox   = new();
    private readonly DropdownListAdapter dropdownList;
    private readonly DropdownCollection  itemCollection;
    private readonly TView               currentItemView = new();
    private readonly TextWidget          placeholder     = new();
    private readonly DropdownOverlay     overlay;
    private          bool                dropdownOpen;
    private          int                 selectedIndex    = -1;
    private          bool                allowNoSelection = true;

    public ICollection<TItemType> Items => itemCollection;

    public event Action<int>? SelectedIndexChanged;

    public int SelectedIndex
    {
        get => selectedIndex;
        set => SetSelectedIndex(value, true);
    }
    
    public string PlaceholderTExt
    {
        get => placeholder.Text;
        set => placeholder.Text = value;
    }
    
    public Dropdown()
    {
        overlay = new DropdownOverlay(this);
        
        dropdownList = new DropdownListAdapter(this);
        
        Children.Add(buttonBox);

        overlay.Content = dropdownBox;
        dropdownBox.Content = dropdownList;
        itemCollection = new DropdownCollection(this);
    }

    protected override Vector2 GetContentSize(Vector2 availableSize)
    {
        Padding buttonPadding = GetVisualStyle().DropdownButtonPadding;
        Vector2 buttonSize = buttonBox.GetCachedContentSize(availableSize);

        return new Vector2(buttonSize.X + buttonPadding.Horizontal, buttonSize.Y + buttonPadding.Vertical);
    }

    protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
    {
        Padding buttonPadding = GetVisualStyle().DropdownButtonPadding;
        buttonBox.UpdateLayout(context, new LayoutRect(availableSpace.Left + buttonPadding.Left, availableSpace.Top + buttonPadding.Top, availableSpace.Width - buttonPadding.Horizontal, availableSpace.Height - buttonPadding.Vertical));

        Vector2 screen = new Vector2(context.PhysicalScreenWidget, context.PhysicalScreenHeight);
        Vector2 scrollViewSize = dropdownBox.GetCachedContentSize(screen);

        scrollViewSize.X = Math.Max(scrollViewSize.X, availableSpace.Width);

        Vector2 origin = new Vector2(availableSpace.Left, availableSpace.Bottom);

        if (origin.Y + scrollViewSize.Y > screen.Y)
        {
            float distance = (origin.Y + scrollViewSize.Y) - screen.Y;
            origin.Y -= distance;
        }

        if (origin.X + scrollViewSize.X > screen.X)
        {
            origin.X = availableSpace.Right - scrollViewSize.X;
        }
        
        overlay.DropdownArea = new LayoutRect(origin.X, origin.Y, scrollViewSize.X, scrollViewSize.Y);
    }

    public void SetItems(IEnumerable<TItemType> itemSource)
    {
        itemCollection.SetItems(itemSource);
    }

    private void SetSelectedIndex(int newIndex, bool refreshItems = true)
    {
        var notify = false;
        if (selectedIndex != newIndex)
            notify = true;

        selectedIndex = newIndex;

        if (notify)
            SelectedIndexChanged?.Invoke(newIndex);

        if (refreshItems)
            dropdownList.UpdateItems();

        UpdateCurrentItem();
    }
    
    private void NotifyItemsChanged()
    {
        if (itemCollection.Count == 0 || allowNoSelection)
        {
            SetSelectedIndex(-1, false);
        }
        else
        {
            SetSelectedIndex(0, false);
        }

        this.dropdownList.UpdateItems();
    }

    private void UpdateCurrentItem()
    {
        if (selectedIndex == -1)
        {
            this.buttonBox.Content = placeholder;
        }
        else
        {
            this.buttonBox.Content = currentItemView;
            currentItemView.UpdateView(itemCollection[selectedIndex]);
        }
    }

    private sealed class DropdownCollection : ICollection<TItemType>
    {
        private readonly List<TItemType>            items = new();
        private readonly Dropdown<TItemType, TView> dropdown;

        public DropdownCollection(Dropdown<TItemType, TView> dropdown)
        {
            this.dropdown = dropdown;
        }

        public IEnumerator<TItemType> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public void SetItems(IEnumerable<TItemType> itemSource)
        {
            this.items.Clear();
            this.items.AddRange(itemSource);
            dropdown.NotifyItemsChanged();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TItemType item)
        {
            items.Add(item);
            dropdown.NotifyItemsChanged();
        }

        public void Clear()
        {
            items.Clear();
            dropdown.NotifyItemsChanged();
        }

        public bool Contains(TItemType item)
        {
            return items.Contains(item);
        }

        public void CopyTo(TItemType[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(TItemType item)
        {
            bool removed = items.Remove(item);
            if (removed)
                dropdown.NotifyItemsChanged();
            return removed;
        }

        public int Count => items.Count;
        public bool IsReadOnly => false;

        public TItemType this[int index] => items[index];
    }

    private class DropdownListAdapter : ListAdapter<ScrollView, DropdownViewHolder<TItemType, TView>>
    {
        private readonly DataHelper<TItemType>      items;
        private readonly Dropdown<TItemType, TView> dropdown;

        public DropdownListAdapter(Dropdown<TItemType, TView> dropdown)
        {
            this.dropdown = dropdown;
            items = new DataHelper<TItemType>(this);
        }

        public void UpdateItems()
        {
            this.items.SetItems(dropdown.itemCollection);
        }
        
        public override DropdownViewHolder<TItemType, TView> CreateViewHolder(int itemIndex, Box rootWidget)
        {
            return new DropdownViewHolder<TItemType, TView>(itemIndex, rootWidget);
        }

        public override void UpdateView(DropdownViewHolder<TItemType, TView> viewHolder)
        {
            var item = items[viewHolder.ItemIndex];
            viewHolder.UpdateView(item, viewHolder.ItemIndex == dropdown.SelectedIndex);
            
            viewHolder.Clicked = OnItemClicked;
        }
        
        void OnItemClicked(int index)
        {
            dropdown.SetSelectedIndex(index, true);
        }
    }

    private sealed class DropdownItemHolder : ContentWidget,
        IMouseClickHandler,
        IMouseEnterHandler,
        IMouseLeaveHandler,
        IMouseDownHandler,
        IMouseUpHandler
    {
        private bool hovered;
        private bool pressed;
        private bool active;

        public bool IsActive
        {
            get => active;
            set
            {
                active = value;
                InvalidateGeometry();
            }
        }
        
        public Action? Callback { get; set; }

        protected override void RebuildGeometry(GeometryHelper geometry)
        {
            GetVisualStyle().DrawDropdownItemBackground(this, geometry, hovered, pressed, active);
        }

        public void OnMouseClick(MouseButtonEvent e)
        {
            if (e.Button != MouseButton.Left)
                return;
            
            e.RequestFocus();
            Callback?.Invoke();
        }

        public void OnMouseEnter(MouseMoveEvent e)
        {
            hovered = true;
            InvalidateGeometry();
        }

        public void OnMouseLeave(MouseMoveEvent e)
        {
            hovered = false;
            InvalidateGeometry();
        }

        public void OnMouseDown(MouseButtonEvent e)
        {
            if (e.Button != MouseButton.Left)
                return;

            e.Handle();
            pressed = true;
            InvalidateGeometry();
        }

        public void OnMouseUp(MouseButtonEvent e)
        {
            if (e.Button != MouseButton.Left)
                return;

            e.Handle();
            pressed = false;
            InvalidateGeometry();
        }
    }
    
    private class DropdownViewHolder<TItemType, TView> : ViewHolder
        where TView : DropdownItemView<TItemType>, new()
    {
        private readonly TView              view   = new();
        private readonly DropdownItemHolder holder = new();

        public Action<int>? Clicked;
        
        public DropdownViewHolder(int itemIndex, Box root) : base(itemIndex, root)
        {
            holder.Content = view;
            root.Content = holder;

            holder.Callback = OnClick;
        }

        public void UpdateView(TItemType data, bool isActiveItem)
        {
            holder.IsActive = isActiveItem;
            view.UpdateView(data);
        }

        private void OnClick()
        {
            Clicked?.Invoke(ItemIndex);
        }
    }

    private void ShowDropdown()
    {
        HideDropdown();

        dropdownOpen = true;

        GuiManager?.TopLevels.Add(overlay);
    }

    private void HideDropdown()
    {
        if (!dropdownOpen)
            return;

        dropdownOpen = false;

        GuiManager?.TopLevels.Remove(overlay);
    }
    
    public void OnMouseClick(MouseButtonEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        e.RequestFocus();
    }

    public void OnFocusGained(FocusEvent e)
    {
        e.Handle();

        ShowDropdown();
    }

    public void OnFocusLost(FocusEvent e)
    {
        HideDropdown();
    }

    private sealed class DropdownOverlay : ContentWidget
    {
        private readonly Dropdown<TItemType, TView> dropdown;
        private          LayoutRect                 dropdownArea;

        public override GuiManager? GuiManager => base.GuiManager ?? dropdown.GuiManager;

        public LayoutRect DropdownArea
        {
            get => dropdownArea;
            set
            {
                dropdownArea = value;
                InvalidateLayout();
            }
        }
        
        public DropdownOverlay(Dropdown<TItemType, TView> dropdown)
        {
            LayoutRoot = this;
            this.dropdown = dropdown;
        }

        protected override Vector2 GetContentSize(Vector2 availableSize)
        {
            return availableSize;
        }

        protected override void ArrangeChildren(IGuiContext context, LayoutRect availableSpace)
        {
            base.ArrangeChildren(context, dropdownArea);
        }

        protected override void RebuildGeometry(GeometryHelper geometry)
        {
            GetVisualStyle().DrawDropdownItemsBackground(geometry, DropdownArea);
        }
    }
}