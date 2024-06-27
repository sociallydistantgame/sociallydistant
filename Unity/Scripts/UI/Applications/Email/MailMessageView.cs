#nullable enable
using System;
using Social;
using TMPro;
using UnityEngine;
using UnityExtensions;
using UnityEngine.UI;

namespace UI.Applications.Email
{
	public sealed class MailMessageView : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI subjectAndSender = null!;

		private Button button;
		private IMailMessage? message;

		public Action<IMailMessage>? Callback { get; set; }
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(MailMessageView));
			this.MustGetComponent(out button);
		}

		private void Start()
		{
			button.onClick.AddListener(OnClick);
		}

		public void UpdateMessage(IMailMessage message)
		{
			this.message = message;

			subjectAndSender.SetText($"<b>{message.Subject}</b>{Environment.NewLine}{message.From.ChatName}");
		}

		private void OnClick()
		{
			if (this.message == null)
				return;
			
			Callback?.Invoke(this.message);
		}
	}
}