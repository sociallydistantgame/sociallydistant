#nullable enable
using ImGuiNET;
using SociallyDistant.GameplaySystems.Chat;

namespace SociallyDistant.DevTools
{
	public sealed class ConversationScriptDebugger : IDevMenu
	{
		/// <inheritdoc />
		public string Name => "Conversation Script Debugger";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			var conversationManager = ConversationManager.Instance;
			
			if (conversationManager == null)
			{
				ImGui.Text("ConversationManager is unavailable.");
				return;
			}

			ImGui.Text("Active Conversations");

			var wereAnyActive = false;
			foreach (string conversationId in conversationManager.ActiveConversations)
			{
				ImGui.Columns(2);

				ImGui.Text(conversationId);
				ImGui.NextColumn();
				if (ImGui.Button("Info"))
				{
					devMenu.PushMenu(new ConversationInfo(conversationManager, conversationId));
					return;
				}
                
				ImGui.Columns(1);
			}
			
			if (!wereAnyActive)
				ImGui.Text("No active conversations!");
			
			ImGui.Text("Actions");
			
			if (ImGui.Button("Start Conversation"))
			{
				devMenu.PushMenu(new StartConversationMenu(conversationManager));
				return;
			}
		}
	}
}