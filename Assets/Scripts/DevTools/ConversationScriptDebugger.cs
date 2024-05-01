#nullable enable
using GameplaySystems.Chat;
using UnityEngine;

namespace DevTools
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
				GUILayout.Label("ConversationManager is unavailable.");
				return;
			}

			GUILayout.Label("Active Conversations");

			var wereAnyActive = false;
			foreach (string conversationId in conversationManager.ActiveConversations)
			{
				GUILayout.BeginHorizontal();

				GUILayout.Label(conversationId);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Info"))
				{
					devMenu.PushMenu(new ConversationInfo(conversationManager, conversationId));
					return;
				}
                
				GUILayout.EndHorizontal();
			}
			
			if (!wereAnyActive)
				GUILayout.Label("No active conversations!");
			
			GUILayout.Label("Actions");
			
			if (GUILayout.Button("Start Conversation"))
			{
				devMenu.PushMenu(new StartConversationMenu(conversationManager));
				return;
			}
		}
	}

	public sealed class ConversationInfo : IDevMenu
	{
		private readonly ConversationManager conversationManager;
		private readonly string id;

		private string chooseIdentifier = string.Empty;

		/// <inheritdoc />
		public string Name => $"Conversation info: {id}";

		public ConversationInfo(ConversationManager conversationManager, string id)
		{
			this.conversationManager = conversationManager;
			this.id = id;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			GUILayout.Label($"Is Active: {conversationManager.IsConversationActive(id)}");
			
			GUILayout.Label("Debug Actions");
			GUILayout.Label("Mark Branch as Chosen (enter the identifier stated in the script)");
			chooseIdentifier = GUILayout.TextField(chooseIdentifier);
			if (GUILayout.Button("MARK"))
			{
				conversationManager.ChooseBranch(id, chooseIdentifier);
				chooseIdentifier = string.Empty;
			}
		}
	}
}