#nullable enable
using SociallyDistant.Core.Chat;
using SociallyDistant.Core.Social;

namespace SociallyDistant.GameplaySystems.Chat
{
	public sealed class InitialInteraction : IBranchDefinition
	{
		private readonly ConversationManager conversationManager;
		private readonly string scriptId;
		
		/// <inheritdoc />
		public IProfile Target { get; }

		/// <inheritdoc />
		public string Message { get; }

		public InitialInteraction(ConversationManager conversationManager, IProfile target, string scriptId, string message)
		{
			this.conversationManager = conversationManager;
			this.Target = target;
			this.Message = message;
			this.scriptId = scriptId;
		}
		
		/// <inheritdoc />
		public void Pick()
		{
			conversationManager.TryStartConversation(scriptId);
		}
	}
}