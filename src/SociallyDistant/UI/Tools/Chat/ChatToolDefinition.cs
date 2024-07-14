using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ChatToolDefinition : ITabbedToolDefinition
{
    public IProgram Program { get; } = new ChatProgram();
    public bool AllowUserTabs { get; } = false;
    public INotificationGroup? NotificationGroup { get; } = null;
}