using Social;
using UnityEngine;

namespace UI.Applications.Chat
{
	public class DirectMessageModel
	{
		public Texture2D Avatar { get; set; }
		public string DisplayName { get; set; }
		public string ExtraInfo { get; set; }
		public IDirectConversation Conversation { get; set; }
	}
}