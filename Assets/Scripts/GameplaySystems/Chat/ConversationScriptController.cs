#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;
using System.Threading.Tasks;
using Chat;
using Core;
using Social;
using Steamworks;
using TMPro;
using UnityEngine;

namespace GameplaySystems.Chat
{
	public sealed class ConversationScriptController : IConversationController
	{
		private readonly ConversationManager conversationManager;
		private readonly Dictionary<IProfile, List<BranchDefinition>> branchDefinitions = new();
		private readonly List<string> chosenBranches = new();
		private readonly List<TaskCompletionSource<string>> branchAwaiters = new();
		private readonly INarrativeThread narrativeThread;
		
		/// <inheritdoc />
		public IWorldManager WorldManager { get; }

		/// <inheritdoc />
		public ISocialService SocialService { get; }

		/// <inheritdoc />
		public IChatConversation Conversation { get; }

		public ConversationScriptController(ConversationManager conversationManager, IWorldManager worldManager, ISocialService socialService, IChatConversation conversation, INarrativeThread narrativeThread)
		{
			this.conversationManager = conversationManager;
			this.WorldManager = worldManager;
			this.SocialService = socialService;
			this.Conversation = conversation;
			this.narrativeThread = narrativeThread;
		}

		/// <inheritdoc />
		public bool IsBranchChosen(string identifier)
		{
			return chosenBranches.Contains(identifier);
		}

		/// <inheritdoc />
		public void ChooseBranch(string definitionId)
		{
			Debug.Log($"[CONVERSATION SCRIPT/{Conversation.Id}] <New interaction> Player chose branch {definitionId}");
			
			// We do this just to make sure we keep any parallel branch wait operations
			// in sync.
			TaskCompletionSource<string>[] awaiterArray = branchAwaiters.ToArray();
			branchAwaiters.Clear();
			
			this.chosenBranches.Add(definitionId);
			
			// Prevent choices from the previous branch from being picked again.
			// The script system doesn't support backtracking within the same conversation.
			this.branchDefinitions.Clear();
			this.conversationManager.RefreshBranches(this.narrativeThread.ChannelId);

			foreach (TaskCompletionSource<string> awaiter in awaiterArray)
			{
				awaiter.SetResult(definitionId);
			}
		}

		/// <inheritdoc />
		public async Task Say(IProfile profile, string message)
		{
			Debug.Log($"[CONVERSATION SCRIPT/{Conversation.Id}] <{profile.ChatUsername} says> {message}");

			if (profile == this.SocialService.PlayerProfile)
			{
				ChatBoxController chatBox = await conversationManager.RequestChatBoxAccess(this.narrativeThread.ChannelId);
				
				var stringBuilder = new StringBuilder(message.Length);
				stringBuilder.Length = 0;
				
				chatBox.SetTExt(stringBuilder);
				
				for (var i = 0; i < message.Length; i++)
				{
					char nextChar = message[i];

					await Task.Delay(120);
					stringBuilder.Append(nextChar);
					chatBox.SetTExt(stringBuilder);
				}
				
				await Task.Delay(240);

				stringBuilder.Length = 0;
				chatBox.SetTExt(stringBuilder);

				chatBox.ReleaseControl();
			}
			
			await narrativeThread.Say(profile, message);
		}

		/// <inheritdoc />
		public void DeclareBranch(IProfile target, string id, string message)
		{
			Debug.Log($"[CONVERSATION SCRIPT/{Conversation.Id}] <New branch defined> {id}");
			if (!branchDefinitions.ContainsKey(target))
				branchDefinitions[target] = new List<BranchDefinition>();

			this.branchDefinitions[target].Add(new BranchDefinition(this, target, id, message));
			this.conversationManager.RefreshBranches(this.narrativeThread.ChannelId);
		}

		/// <inheritdoc />
		public Task<string> WaitForNextBranch()
		{
			Debug.Log($"[CONVERSATION SCRIPT/{Conversation.Id}] Waiting for the player to choose a branch...");
			
			var source = new TaskCompletionSource<string>();

			branchAwaiters.Add(source);

			return source.Task;
		}

		/// <inheritdoc />
		public async Task SendMission(IProfile profile, string missionId)
		{
			await this.narrativeThread.AttachMission(profile, missionId);
		}

		/// <inheritdoc />
		public IEnumerable<IBranchDefinition> GetBranches()
		{
			foreach (List<BranchDefinition> list in branchDefinitions.Values)
			{
				foreach (BranchDefinition branch in list)
				{
					yield return branch;
				}
			}
		}
	}
}