#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public class ChatConversationController : MonoBehaviour
	{
		[SerializeField]
		private ChatMessageListView listView = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ChatConversationController));
		}

		public void SetMessageList(IList<ChatMessageModel> messageList)
		{
			this.listView.SetItems(messageList);
		}
	}
}