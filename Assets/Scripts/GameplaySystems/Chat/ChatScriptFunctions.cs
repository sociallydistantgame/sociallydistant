#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using Chat;
using Core.Scripting;
using Social;

namespace GameplaySystems.Chat
{
	public sealed class ChatScriptFunctions : ScriptModule
	{
		private readonly IConversationController controller;

		private IProfile currentActor = null!;

		public ChatScriptFunctions(IConversationController controller)
		{
			this.controller = controller;

			this.Possess(this.controller.Conversation.ActorIds.First());
		}

		[Function("actor")]
		public void Possess(string narrativeId)
		{
			if (narrativeId != "player")
			{
				if (!controller.Conversation.ActorIds.Contains(narrativeId))
					throw new InvalidOperationException($"Narrative ID {narrativeId} was not declared in the script's list of actors, and so cannot be possessed.");
			}


			currentActor = controller.SocialService.GetNarrativeProfile(narrativeId);
		}

		[Function("say")]
		public async Task Say(string message)
		{
			await controller.Say(currentActor, message);
		}

		[Function("mention")]
		public string Mention(string narrativeId)
		{
			IProfile user = controller.SocialService.GetNarrativeProfile(narrativeId);

			var uriBuilder = new UriBuilder()
			{
				Scheme = "chat",
				Host = "profile",
				Path = user.ChatUsername
			};

			return $"<link=\"{uriBuilder.Uri}\">@{user.ChatUsername}</link>";
		}

		[Function("mission")]
		public async Task SendMission(string missionId)
		{
			await controller.SendMission(currentActor, missionId);
		}
		
		[Function("branch")]
		public void DeclareBranch(string id, string messageText)
		{
			controller.DeclareBranch(currentActor, id, messageText);
		}

		[Function("branch_picked")]
		public int IsBranchPicked(string identifier)
		{
			return controller.IsBranchChosen(identifier) ? 0 : 1;
		}

		[Function("wait_for_choices")]
		public Task<string> WaitForChoices()
		{
			return controller.WaitForNextBranch();
		}

		[Function("shrug")]
		public async Task Shrug()
		{
			await Say("\u00af\\_(ツ)_/\u00af");
		}

		[Function("action")]
		public async Task Action(string message)
		{
			await Say($"<i> * {message} * </i>");
		}
	}
}