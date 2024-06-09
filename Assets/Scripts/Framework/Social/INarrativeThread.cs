using System.Threading.Tasks;
using Core;

namespace Social
{
	public interface INarrativeThread
	{
		ObjectId ChannelId { get; }
		
		Task Say(IProfile sender, string textMessage);
		Task AttachMission(IProfile sender, string missionId);
	}
}