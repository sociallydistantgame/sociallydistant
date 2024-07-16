#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using AcidicGui.Widgets;
using Core;
using Core.WorldData.Data;
using GamePlatform;
using GameplaySystems.Chat;
using GameplaySystems.Social;
using Shell.Common;
using Social;
using TMPro;
using UI.PlayerUI;
using UI.Widgets;
using UI.Widgets.Settings;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UniRx;

namespace UI.Applications.Chat
{
	public class ChatApplicationController : MonoBehaviour{
		
		private void Awake()
		{
			worldManager = GameManager.Instance.WorldManager;
			socialService = GameManager.Instance.SocialService;
			conversationManager = ConversationManager.Instance;

			notificationGroup = GameManager.Instance.NotificationManager.GetNotificationGroup(NotificationGroups.Chat);
			
			this.AssertAllFieldsAreSerialized(typeof(ChatApplicationController));
			this.MustGetComponentInParent(out uiManager);
		}

		private void OnEnable()
		{
			if (conversationManager != null)
			{
			}

			if (currentChannel != null)
			{
				userMessages.Clear();

				foreach (IUserMessage? message in currentChannel.Messages)
				{
					userMessages.Add(message);
					notificationGroup.MarkNotificationAsRead(message.Id);
				}

				UpdateMessageList();

				this.messageSendObserver = currentChannel.SendObservable.Subscribe(OnMessageReceived);
				this.messageDeleteObserver = currentChannel.EditObservable.Subscribe(OnMessageEdit);
				this.messageDeleteObserver = currentChannel.DeleteObservable.Subscribe(OnMessageDelete);
			}
		}

		private void OnDisable()
		{
			this.messageSendObserver?.Dispose();
            this.messageSendObserver = null;
			this.messageDeleteObserver?.Dispose();
            this.messageDeleteObserver = null;
			this.messageDeleteObserver?.Dispose();
            this.messageDeleteObserver = null;
			this.pendingChatBoxRequestsObserver?.Dispose();
		}

		private void OnDestroy()
		{
			messageSendObserver?.Dispose();
			messageDeleteObserver?.Dispose();
			messageModifyObserver?.Dispose();
			guildJoinObserver?.Dispose();
			guildLeaveObserver?.Dispose();
			branchObserver?.Dispose();
			pendingChatBoxRequestsObserver?.Dispose();
			typingObserver?.Dispose();
		}
		
		private void OnHomeButtonToggle(bool isOn)
		{
			if (isOn)
				ShowDirectMessagesList();
		}
	}
}