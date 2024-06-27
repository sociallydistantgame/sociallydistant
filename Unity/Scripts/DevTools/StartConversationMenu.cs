#nullable enable
using GameplaySystems.Chat;
using UnityEngine;

namespace DevTools
{
	public sealed class StartConversationMenu : IDevMenu
	{
		private readonly ConversationManager conversationManager;
		
		/// <inheritdoc />
		public string Name => "Start Conversation";

		public StartConversationMenu(ConversationManager conversationManager)
		{
			this.conversationManager = conversationManager;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			foreach (string id in conversationManager.AllConversationIds)
			{
				if (!conversationManager.IsConversationActive(id) && GUILayout.Button(id))
				{
					conversationManager.TryStartConversation(id);
					devMenu.PopMenu();
					return;
				}
			}
		}
	}
}