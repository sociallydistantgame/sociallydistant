#nullable enable

using System.Collections.Generic;
using System.Threading;
using ContentManagement;
using System.Threading.Tasks;
using Core;
using Social;

namespace Chat
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