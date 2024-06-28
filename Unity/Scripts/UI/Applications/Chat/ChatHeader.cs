#nullable enable
using System;
using TMPro;
using UI.Widgets.Settings;
using UnityEngine;
using UnityExtensions;
using UnityEngine.UI;

namespace UI.Applications.Chat
{
	public class ChatHeader : MonoBehaviour
	{
		[Header("UI")]
		
		private TextMeshProUGUI channelTitleText = null!;

		
		private TextMeshProUGUI channelDescriptionText = null!;

		
		private RectTransform guildIconArea = null!;

		
		private RectTransform userIconArea = null!;

		
		private AvatarWidgetController userImage = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ChatHeader));
		}

		public void DisplayGuildHeader(string channelName, string channelDescription)
		{
			this.userIconArea.gameObject.SetActive(false);
			this.guildIconArea.gameObject.SetActive(true);
			
			this.channelTitleText.SetText(channelName);
			this.channelDescriptionText.SetText(channelDescription);
		}

		public void DisplayDirectMessage(string displayName, string username, Texture2D? userAvatar)
		{
			this.userIconArea.gameObject.SetActive(true);
			this.guildIconArea.gameObject.SetActive(false);

			this.userImage.DefaultAvatarColor = Color.yellow;
			this.userImage.AvatarTexture = userAvatar;
			this.userImage.UpdateUI();
			
			this.channelTitleText.SetText("Direct messages");
			this.channelDescriptionText.SetText($"<b>{displayName}</b> {username}");
		}
	}
}