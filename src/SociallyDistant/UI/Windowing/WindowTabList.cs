using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Windowing;

public sealed class WindowTabList : Widget
{
    private readonly WrapPanel        wrapPanel   = new();
    private readonly List<WindowTab>  views       = new();
    private readonly List<Definition> definitions = new();

    private CommonColor color;
    private bool showNewTab = false;

    public CommonColor Color
    {
        get => color;
        set
        {
            color = value;
            UpdateViews();
        }
    }
    
    public WindowTabList()
    {
        Children.Add(wrapPanel);

        wrapPanel.Direction = Direction.Horizontal;
    }

    public ITabDefinition CreateTab()
    {
        var definition = new Definition(this);

        definitions.Add(definition);
        
        UpdateViews();
        
        return definition;
    }

    public void RemoveTab(ITabDefinition definition)
    {
        if (definition is not Definition actualDefinition)
            return;

        definitions.Remove(actualDefinition);
        UpdateViews();
    }
    
    private void UpdateViews()
    {
        while (views.Count > definitions.Count)
        {
            wrapPanel.ChildWidgets.Remove(views[^1]);
            views.RemoveAt(views.Count-1);
        }

        while (views.Count < definitions.Count)
        {
            var view = new WindowTab();
            views.Add(view);
            wrapPanel.ChildWidgets.Add(view);
        }

        for (var i = 0; i < definitions.Count; i++)
        {
            Definition definition = definitions[i];
            WindowTab view = views[i];

            view.SetCustomProperty(color);
            view.Title = definition.Title;
            view.Active = definition.Active;
            view.Closeable = definition.Closeable;
        }
    }
    
    internal class Definition : ITabDefinition
    {
        private readonly WindowTabList list;
        private string title = "Tab title";
        private bool closeable;
        private bool active;

        public Definition(WindowTabList list)
        {
            this.list = list;
        }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                list.UpdateViews();
            }
        }

        public bool Closeable
        {
            get => closeable;
            set
            {
                closeable = value;
                list.UpdateViews();
            }
        }

        public bool Active
        {
            get => active;
            set
            {
                active = value;
                list.UpdateViews();
            }
        }
    }
}