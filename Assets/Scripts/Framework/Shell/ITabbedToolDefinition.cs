using ContentManagement;
using Shell.Common;

namespace Shell
{
	public interface ITabbedToolDefinition : IGameContent
	{
		IProgram Program { get; }
		bool AllowUserTabs { get; }
		INotificationGroup? NotificationGroup { get; }
	}
}