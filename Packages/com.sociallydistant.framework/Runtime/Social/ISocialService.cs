#nullable enable

using System.Collections.Generic;

namespace Social
{
	public interface ISocialService
	{
		IProfile PlayerProfile { get; }

		IEnumerable<IProfile> GetFriends(IProfile user);
		IEnumerable<IProfile> GetFollowers(IProfile user);
		IEnumerable<IProfile> GetFollowing(IProfile user);
		IEnumerable<IProfile> GetBlockedProfiles(IProfile user);

		IEnumerable<IGuild> GetGuilds(IProfile user);
		IEnumerable<IDirectConversation> GetDirectConversations(IProfile user);

		IEnumerable<IUserMessage> GetSocialPosts(IProfile profile);
		IEnumerable<IUserMessage> GetTimeline(IProfile profile);
	}
}