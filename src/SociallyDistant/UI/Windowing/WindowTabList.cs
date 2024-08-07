using System.ComponentModel;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.UI.Common;

namespace SociallyDistant.UI.Windowing;

public sealed class WindowTabList : Widget
{
    private readonly WrapPanel           wrapPanel    = new();
    private readonly List<WindowTab>     views        = new();
    private readonly List<Definition>    definitions  = new();
    private readonly Button              newTabButton = new();
    private readonly CompositeIconWidget newTabIcon   = new();
    private          Action?             newTabCallback;

    private CommonColor color;
    private bool showNewTab = false;

    public ITabDefinition this[int index] => definitions[index];

    public Action? NewTabCallback
    {
        get => newTabCallback;
        set => newTabCallback = value;
    }

    public bool ShowNewTab
    {
        get => newTabButton.Visibility == Visibility.Visible;
        set
        {
            newTabButton.Visibility = value
                ? Visibility.Visible
                : Visibility.Collapsed;

            this.UpdateViews();
        }
    }
    
    public CommonColor Color
    {
        get => color;
        set
        {
            color = value;
            UpdateViews();
        }
    }

    public event Action<int>? TabClicked;
    
    public WindowTabList()
    {
        Children.Add(wrapPanel);

        wrapPanel.Direction = Direction.Horizontal;

        newTabButton.Content = newTabIcon;
        newTabIcon.Icon = MaterialIcons.Add;
        this.newTabButton.Clicked += OnNewTab; 
    }

    private void OnNewTab()
    {
        this.newTabCallback?.Invoke();
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
        if (newTabButton.Parent != null)
            this.wrapPanel.ChildWidgets.Remove(newTabButton);
        
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
            view.TabIndex = i;
            view.ClickCallback = OnTabClicked;
        }

        this.wrapPanel.ChildWidgets.Add(newTabButton);
    }

    private void OnTabClicked(int index)
    {
        this.TabClicked?.Invoke(index);
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