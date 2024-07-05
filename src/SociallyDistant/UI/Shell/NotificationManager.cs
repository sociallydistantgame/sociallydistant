using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.UI.Shell
{
	public sealed class NotificationManager : 
		INotificationManager,
		IDisposable
	{
		private readonly Dictionary<string, NotificationGroup> groups = new();
		private readonly IWorldManager worldManager;

		public NotificationManager(IWorldManager worldManager)
		{
			this.worldManager = worldManager;
			InstallEvents();
		}
		
		
		/// <inheritdoc />
		public void Dispose()
		{
			NotificationGroup[] groupObjects = groups.Values.ToArray();
			groups.Clear();

			foreach (NotificationGroup group in groupObjects)
				group.Dispose();
			
			UninstallEvents();
		}

		private void InstallEvents()
		{
			worldManager.Callbacks.AddCreateCallback<WorldNotificationData>(OnCreateNotification);
			worldManager.Callbacks.AddDeleteCallback<WorldNotificationData>(OnDeleteNotification);
		}

		private void OnCreateNotification(WorldNotificationData subject)
		{
			GetNotificationGroupInternal(subject.GroupId).CreateInternal(subject);
		}
		
		private void OnDeleteNotification(WorldNotificationData subject)
		{
			GetNotificationGroupInternal(subject.GroupId).DeleteInternal(subject);
		}

		private void UninstallEvents()
		{
			worldManager.Callbacks.RemoveCreateCallback<WorldNotificationData>(OnCreateNotification);
			worldManager.Callbacks.RemoveDeleteCallback<WorldNotificationData>(OnDeleteNotification);
		}

		/// <inheritdoc />
		public INotificationGroup GetNotificationGroup(string groupId)
		{
			return GetNotificationGroupInternal(groupId);
		}

		private NotificationGroup GetNotificationGroupInternal(string id)
		{
			if (!this.groups.TryGetValue(id, out NotificationGroup group))
			{
				group = new NotificationGroup(id, worldManager);
				groups.Add(id, group);
			}

			return group;
		}
	}
}