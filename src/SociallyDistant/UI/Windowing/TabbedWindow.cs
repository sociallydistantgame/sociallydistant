using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Windowing;

public sealed class TabbedWindow : 
    WindowBase, 
    ITile
{
    private readonly List<WindowContent> panels = new();
    private int? activePanel;
    
    internal TabbedWindow(INotifyCloseWorkspace workspace) : base(workspace)
    {
    }

    public IContentPanel? ActiveContent
    {
        get
        {
            if (activePanel != null)
                return panels[activePanel.Value];

            return null;
        }
    }
    public Action? NewTabCallback { get; set; }
    public bool ShowNewTab { get; set; }
    public void Hide()
    {
        Visibility = Visibility.Hidden;
    }

    public void Show()
    {
        Visibility = Visibility.Visible;
    }

    public IReadOnlyList<IContentPanel> ContentPanels => panels;
    public void NextTab()
    {
        throw new NotImplementedException();
    }

    public void PreviousTab()
    {
        throw new NotImplementedException();
    }

    public void SwitchTab(int index)
    {
        throw new NotImplementedException();
    }

    public void CloseTab(int index)
    {
        throw new NotImplementedException();
    }

    public IContentPanel CreateTab()
    {
        activePanel = panels.Count;
        var tab = this.Tabs.CreateTab();
        var panel = new WindowContent(this, tab);

        panels.Add(panel);

        UpdateState();
        
        return panel;
    }

    public bool RemoveTab(IContentPanel panel)
    {
        throw new NotImplementedException();
    }

    private void UpdateState()
    {
        var i = 0;
        foreach (WindowContent panel in panels)
        {
            panel.IsTabActive = i == activePanel;

            if (i == activePanel && Decorations.Client != panel.Content)
            {
                Decorations.Client = panel.Content;
            }
            
            i++;
        }
    }

    private class WindowContent : IContentPanel
    {
        private readonly TabbedWindow window;
        private readonly ITabDefinition tab;

        public bool IsTabActive
        {
            get => tab.Active;
            set => tab.Active = value;
        }
        
        private Widget? content;

        public WindowContent(TabbedWindow window, ITabDefinition tab)
        {
            this.window = window;
            this.tab = tab;
        }

        public bool CanClose
        {
            get => tab.Closeable;
            set => tab.Closeable = value;
        }
        public void Close()
        {
            throw new NotImplementedException();
        }

        public void ForceClose()
        {
            throw new NotImplementedException();
        }

        public IWindow Window => window;

        public string Title
        {
            get => tab.Title;
            set => tab.Title = value;
        }
        
        public CompositeIcon Icon { get; set; }

        public Widget? Content
        {
            get => content;
            set
            {
                content = value;
                window.UpdateState();
            }
        }
    } 
}