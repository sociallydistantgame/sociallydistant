using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Windowing;

public sealed class Window :
    WindowBase,
    IWindowWithClient<Widget?>
{
    private readonly ITabDefinition tab;
    private readonly IContentPanel  contentPanel;

    public IContentPanel ContentPanel => contentPanel;
    
    internal Window(INotifyCloseWorkspace workspace) : base(workspace)
    {
        tab = Tabs.CreateTab();
        tab.Active = true;
        contentPanel=new ContentPanelInstance(this);
    }

    public string Title
    {
        get => tab.Title;
        set => tab.Title = value;
    }
    
    public Widget? Client => Decorations.Client;
    public void SetClient(Widget? newClient)
    {
        Decorations.Client = newClient;
    }

    private sealed class ContentPanelInstance : IContentPanel
    {
        private readonly Window window;
        
        public ContentPanelInstance(Window window)
        {
            
        }

        public bool CanClose
        {
            get => window.CanClose;
            set => window.CanClose = value;
        }
        public void Close()
        {
            window.Close();
        }

        public void ForceClose()
        {
            window.ForceClose();
        }

        public IWindow Window => window;
        public string Title
        {
            get => window.Title;
            set => window.Title = value;
        }

        public CompositeIcon Icon
        {
            get => window.Icon;
            set => window.Icon = value;
        }

        public Widget? Content
        {
            get => window.Client;
            set => window.SetClient(value);
        }
    }
}