using System;
using Core.WorldData.Data;
using UnityEngine;

namespace UI.Applications.Chat
{
	public class ChatMessageModel
	{
		public Texture2D? Avatar;
		public bool UseBubbleStyle;
		public bool IsFromPlayer;
		public string DisplayName = string.Empty;
		public string Username = string.Empty;
		public string FormattedDateTime = string.Empty;
		public DocumentElement[] DocumentData = Array.Empty<DocumentElement>();
	}
}