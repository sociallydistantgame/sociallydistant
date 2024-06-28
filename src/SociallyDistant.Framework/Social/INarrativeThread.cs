using SociallyDistant.Core.Core;

namespace SociallyDistant.Core.Social
{
	public interface INarrativeThread
	{
		ObjectId ChannelId { get; }
		
		Task Say(IProfile sender, string textMessage);
		Task AttachMission(IProfile sender, string missionId);
	}
}