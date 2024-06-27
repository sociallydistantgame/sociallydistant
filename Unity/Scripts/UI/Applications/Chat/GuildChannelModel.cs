using Social;

namespace UI.Applications.Chat
{
	public class GuildChannelModel
	{
		public string ChannelName { get; set; }
		public IChatChannel? Channel { get; set; }
	}
}