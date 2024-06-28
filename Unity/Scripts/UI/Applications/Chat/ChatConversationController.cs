#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

namespace UI.Applications.Chat
{
	public class ChatConversationController : MonoBehaviour
	{
		
		private ChatMessageListView listView = null!;

		private float scrollDelay = 0.25f;
		private float counter = 0;
		private int messageCount = 0;
		private bool scrollToEnd;
		private bool scrolling;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ChatConversationController));
		}

		public void SetMessageList(IList<ChatMessageModel> messageList)
		{
			this.listView.SetItems(messageList);
			messageCount = messageList.Count;
			if (messageCount != 0)
				listView.ScrollTo(messageCount-1, 0f);
			
			scrollToEnd = true;
			counter = 0;
		}
	}
}