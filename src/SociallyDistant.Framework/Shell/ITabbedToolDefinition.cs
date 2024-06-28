using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.Core.Shell
{
	public interface ITabbedToolDefinition : IGameContent
	{
		IProgram Program { get; }
		bool AllowUserTabs { get; }
		INotificationGroup? NotificationGroup { get; }
	}
}