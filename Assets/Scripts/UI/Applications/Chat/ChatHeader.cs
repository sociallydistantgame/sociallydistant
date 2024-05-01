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
		[SerializeField]
		private TextMeshProUGUI channelTitleText = null!;

		[SerializeField]
		private TextMeshProUGUI channelDescriptionText = null!;

		[SerializeField]
		private RectTransform guildIconArea = null!;

		[SerializeField]
		private RectTransform userIconArea = null!;

		[SerializeField]
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