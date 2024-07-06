using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Windowing;

internal interface INotifyCloseWorkspace : IWorkspaceDefinition
{
    void OnCloseWindow(WindowBase window);
}