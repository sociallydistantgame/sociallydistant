#nullable enable

using SociallyDistant.Core.Core;

namespace SociallyDistant.Core.Social
{
	public interface ISocialService
	{
		IProfile PlayerProfile { get; }

		IEnumerable<IProfile> GetFriends(IProfile user);
		IEnumerable<IProfile> GetFollowers(IProfile user);
		IEnumerable<IProfile> GetFollowing(IProfile user);
		IEnumerable<IProfile> GetBlockedProfiles(IProfile user);

		IGuildList GetGuilds();
		IEnumerable<IDirectConversation> GetDirectConversations(IProfile user);

		IEnumerable<IProfile> Profiles { get; }
		
		IEnumerable<IUserMessage> GetSocialPosts(IProfile profile);
		IEnumerable<IUserMessage> GetTimeline(IProfile profile);

		IProfile GetProfileById(ObjectId id);
		IProfile GetNarrativeProfile(string narrativeIdentifier);
		INarrativeThread? GetNarrativeThread(NarrativeThread threadType, string guildId, string channelId, bool create, params string[] nonPlayerActors);
		INewsManager News { get; }
	}
}