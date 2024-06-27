#nullable enable
using System;
using Core;

namespace Shell.Common
{
	public interface INotificationGroup
	{
		string Name { get; }

		IDisposable ObserveUnread(Action<bool> callback);

		void MarkNotificationAsRead(ObjectId correlationId);
	}
}