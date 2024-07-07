using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Windowing;

public sealed class Window :
    WindowBase,
    IWindowWithClient<Widget?>
{
    private readonly ITabDefinition tab;
    
    internal Window(INotifyCloseWorkspace workspace) : base(workspace)
    {
        tab = Tabs.CreateTab();
        tab.Active = true;
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
}