using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.UI.Tools.WebBrowser;

public sealed class WebBrowserTool : ITabbedToolDefinition
{
    public IProgram Program { get; } = new WebBrowserProgram();
    public bool AllowUserTabs { get; } = true;
    public INotificationGroup? NotificationGroup { get; } = null;
}