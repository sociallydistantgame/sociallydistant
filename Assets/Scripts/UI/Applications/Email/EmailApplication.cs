#nullable enable

using System;
using Architecture;
using GameplaySystems.Mail;
using OS.Devices;
using Shell.Windowing;
using UnityEngine;
using UnityExtensions;
using System.Collections.Generic;
using System.Linq;
using GamePlatform;
using GameplaySystems.Social;
using Social;
using TMPro;

namespace UI.Applications.Email
{
	public class EmailApplication : 
		MonoBehaviour,
		IProgramOpenHandler
	{
		[SerializeField]
		private InboxMode inboxMode;

		[SerializeField]
		private TextMeshProUGUI myUserName = null!;

		[SerializeField]
		private TextMeshProUGUI myEmailAddress = null!;

		[SerializeField]
		private MailMessageListView messageList = null!;

		[SerializeField]
		private TextMeshProUGUI currentSubject = null!;

		[SerializeField]
		private TextMeshProUGUI messageStatus = null!;

		[SerializeField]
		private MailConversationView conversation = null!;
		
		private IMailMessage? currentMessage = null;
		private IWindow window = null!;
		private ISystemProcess process = null!;
		private MailManager? mailManager = null!;
		private ISocialService socialService;
		
		private void Awake()
		{
			socialService = GameManager.Instance.SocialService;
			mailManager = MailManager.Instance;
			
			this.AssertAllFieldsAreSerialized(typeof(EmailApplication));
			this.MustGetComponentInParent(out window);
		}

		private void Start()
		{
			this.messageList.ItemSelected += ViewMessage;
			
			UpdateScreen();
		}

		private void UpdateScreen()
		{
			if (mailManager == null)
				return;
			
			IProfile user = socialService.PlayerProfile;

			IEnumerable<IMailMessage> messagesToList = this.inboxMode switch
			{
				InboxMode.Inbox => this.mailManager.GetMessagesForUser(user),
				InboxMode.Outbox => mailManager.GetMessagesFromUser(user),
				InboxMode.CompletedMissions => Enumerable.Empty<IMailMessage>()
			};
			
			myUserName.SetText(user.ChatName);
			myEmailAddress.SetText(user.ChatUsername);
			messageList.SetItems(messagesToList);

			currentSubject.SetText(currentMessage?.Subject ?? string.Empty);
			conversation.ViewMessage(this.currentMessage);
		}
		
		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args)
		{
			this.process = process;
		}

		private void ViewMessage(IMailMessage message)
		{
			this.currentMessage = message;
			this.UpdateScreen();
		}
		
		private enum InboxMode
		{
			Inbox,
			Outbox,
			CompletedMissions
		}
	}
}