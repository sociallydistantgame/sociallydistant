#nullable enable
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.Architecture
{
	public class MainToolGroup : ITabbedToolDefinition
	{
		
		private IProgram program = null!;

		
		private bool allowUserTabs;

		
		private string notificationGroupId = string.Empty;

		public IProgram Program => program;
		public bool AllowUserTabs => allowUserTabs;

		/// <inheritdoc />
		public INotificationGroup? NotificationGroup
		{
			get
			{
#if UNITY_EDITOR

				if (!EditorApplication.isPlaying)
					return null;

#endif

				if (string.IsNullOrWhiteSpace(notificationGroupId))
					return null;

				return SociallyDistantGame.Instance.NotificationManager.GetNotificationGroup(notificationGroupId);
			}
		}
	}
	
	
}