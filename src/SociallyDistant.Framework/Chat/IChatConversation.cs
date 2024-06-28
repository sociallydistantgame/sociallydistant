#nullable enable

using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Social;

namespace SociallyDistant.Core.Chat
{
	public interface IChatConversation : IGameContent
	{
		string Id { get; }
		IEnumerable<string> ActorIds { get; }
		string StartMessage { get; }
		bool IsRepeatable { get; }
		ChatScriptType Type { get; }
		ChatStartType StartType { get; }
		string ChannelId { get; }
		string GuildId { get; }
		
		bool CheckConditions(IWorldManager world, ISocialService socialService);
		Task StartConversation(CancellationToken token, IConversationController controller);
	}
}