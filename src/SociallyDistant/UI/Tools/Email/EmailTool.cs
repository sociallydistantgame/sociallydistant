using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.UI.Tools.Email;

public sealed class EmailTool : ITabbedToolDefinition
{
    public IProgram Program { get; } = new EmailProgram();
    public bool AllowUserTabs { get; } = false;
    public INotificationGroup? NotificationGroup { get; } = null;
}