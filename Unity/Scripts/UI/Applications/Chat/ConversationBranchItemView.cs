#nullable enable
using System;
using Chat;
using TMPro;
using UI.Widgets.Settings;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public sealed class ConversationBranchItemView : MonoBehaviour
	{
		[SerializeField]
		private AvatarWidgetController avatar = null!;
		
		[SerializeField]
		private TextMeshProUGUI characterName = null!;

		[SerializeField]
		private TextMeshProUGUI messageTExt = null!;
		
		private IBranchDefinition? currentBranch;
		private Button button = null!;
		
		public Action<IBranchDefinition>? ClickHandler { get; set; }

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ConversationBranchItemView));
			this.MustGetComponent(out button);
		}

		private void Start()
		{
			button.onClick.AddListener(OnClick);
		}

		public void UpdateView(IBranchDefinition definition)
		{
			currentBranch = definition;

			if (currentBranch != null)
			{
				characterName.SetText(currentBranch.Target.ChatName.ToUpper());
				messageTExt.SetText(currentBranch.Message);
			}
			else
			{
				this.avatar.AvatarTexture = null;
				this.avatar.DefaultAvatarColor = Color.yellow;
				this.avatar.UpdateUI();
				
				this.characterName.SetText(string.Empty);
				this.messageTExt.SetText(string.Empty);
			}
		}

		private void OnClick()
		{
			if (this.currentBranch == null)
				return;
			
			ClickHandler?.Invoke(this.currentBranch);
		}
	}
}