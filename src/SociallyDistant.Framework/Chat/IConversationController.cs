﻿#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Social;

namespace SociallyDistant.Core.Chat
{
	public interface IConversationController
	{
		IWorldManager WorldManager { get; }
		ISocialService SocialService { get; }
		IChatConversation Conversation { get; }

		bool IsBranchChosen(string identifier);

		void ChooseBranch(string definitionId);
		Task Say(IProfile profile, string message);
		void DeclareBranch(IProfile target, string id, string message);

		Task<string> WaitForNextBranch();
		Task SendMission(IProfile profile, string missionId);
		
		IEnumerable<IBranchDefinition> GetBranches();
	}
}