using UnityEngine;

namespace UI.Applications.Chat
{
	public class ServerMember
	{
		public string Username { get; set; } = string.Empty;
		public string DisplayName { get; set; } = string.Empty;
		public Texture2D? Avatar { get; set; }
	}
}