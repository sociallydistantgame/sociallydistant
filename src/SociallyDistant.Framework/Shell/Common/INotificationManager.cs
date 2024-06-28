#nullable enable
namespace SociallyDistant.Core.Shell.Common
{
	public interface INotificationManager
	{
		INotificationGroup GetNotificationGroup(string groupId);
	}
}