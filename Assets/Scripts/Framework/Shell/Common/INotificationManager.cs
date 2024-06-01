#nullable enable
namespace Shell.Common
{
	public interface INotificationManager
	{
		INotificationGroup GetNotificationGroup(string groupId);
	}
}