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
		

		
		private TextMeshProUGUI myUserName = null!;

		
		private TextMeshProUGUI myEmailAddress = null!;

		
		private MailMessageListView messageList = null!;

		
		private TextMeshProUGUI currentSubject = null!;

		
		private TextMeshProUGUI messageStatus = null!;

		
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
	}
}