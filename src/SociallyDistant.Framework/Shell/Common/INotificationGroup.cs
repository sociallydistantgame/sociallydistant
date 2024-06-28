#nullable enable
using SociallyDistant.Core.Core;

namespace SociallyDistant.Core.Shell.Common
{
	public interface INotificationGroup
	{
		string Name { get; }

		IDisposable ObserveUnread(Action<bool> callback);

		void MarkNotificationAsRead(ObjectId correlationId);
	}
}