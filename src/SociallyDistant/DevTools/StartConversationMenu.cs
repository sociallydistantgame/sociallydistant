#nullable enable
using ImGuiNET;
using SociallyDistant.GameplaySystems.Chat;

namespace SociallyDistant.DevTools
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
				if (!conversationManager.IsConversationActive(id) && ImGui.Button(id))
				{
					conversationManager.TryStartConversation(id);
					devMenu.PopMenu();
					return;
				}
			}
		}
	}
}