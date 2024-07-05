using System.Reactive.Subjects;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.UI.Shell
{
	public sealed class NotificationGroup : 
		INotificationGroup,
		IDisposable
	{
		private readonly Subject<bool> unreadObservable = new();
		private readonly IWorldManager worldManager;
		private readonly List<ObjectId> unreadIds = new();
		private readonly Dictionary<ObjectId, int> correlations = new();
		
		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public IDisposable ObserveUnread(Action<bool> callback)
		{
			callback?.Invoke(this.unreadIds.Count > 0);
			return unreadObservable.Subscribe(callback);
		}

		/// <inheritdoc />
		public void MarkNotificationAsRead(ObjectId correlationId)
		{
			if (!correlations.TryGetValue(correlationId, out int index))
				return;

			ObjectId notificationId = unreadIds[index];

			WorldNotificationData data = worldManager.World.Notifications[notificationId];
			
			worldManager.World.Notifications.Remove(data);
		}

		internal NotificationGroup(string groupId, IWorldManager worldManager)
		{
			this.Name = groupId;
			this.worldManager = worldManager;
		}
		
		/// <inheritdoc />
		public void Dispose()
		{
		}

		internal void CreateInternal(WorldNotificationData data)
		{
			correlations.Add(data.CorrelationId, unreadIds.Count);
			unreadIds.Add(data.InstanceId);

			unreadObservable.OnNext(unreadIds.Count > 0);
		}

		internal void DeleteInternal(WorldNotificationData data)
		{
			int index = correlations[data.CorrelationId];

			correlations.Remove(data.CorrelationId);
			unreadIds.RemoveAt(index);

			foreach (ObjectId correlationId in correlations.Keys.ToArray())
			{
				if (correlations[correlationId] > index)
					correlations[correlationId]--;
			}
			
			unreadObservable.OnNext(unreadIds.Count > 0);
		}
	}
}