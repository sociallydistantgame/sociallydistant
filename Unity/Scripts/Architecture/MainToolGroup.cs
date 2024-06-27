#nullable enable
using GamePlatform;
using Shell;
using Shell.Common;
using UI.Shell;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu(menuName = "ScriptableObject/Assets/Main Tool Group")]
	public class MainToolGroup : 
		ScriptableObject, 
		ITabbedToolDefinition
	{
		[SerializeField]
		private UguiProgram program = null!;

		[SerializeField]
		private bool allowUserTabs;

		[SerializeField]
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

				return GameManager.Instance.NotificationManager.GetNotificationGroup(notificationGroupId);
			}
		}
	}
	
	
}