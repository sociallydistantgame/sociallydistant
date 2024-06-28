using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.Core.Social
{
	public interface IChatChannel
	{
		string? NarrativeId { get; }
		ObjectId Id { get; }
		string Name { get; }
		string Description { get; }
		IEnumerable<IProfile> TypingUsers { get; }
		
		IObservable<IUserMessage> SendObservable { get; }
		IObservable<IUserMessage> EditObservable { get; }
		IObservable<IUserMessage> DeleteObservable { get; }
		
		
		MessageChannelType ChannelType { get; }
		ObjectId? GuildId { get; }
		
		IEnumerable<IUserMessage> Messages { get; }

		IDisposable ObserveTypingUsers(Action<IEnumerable<IProfile>> callback);

		ChannelIconData GetIcon();
	}
}