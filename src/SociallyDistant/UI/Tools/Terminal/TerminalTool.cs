using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.UI.Tools.Terminal;

internal class TerminalTool : ITabbedToolDefinition
{
    public IProgram Program => new TerminalProgram();
    public bool AllowUserTabs => true;
    public INotificationGroup? NotificationGroup => null;
}