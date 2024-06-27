namespace UI.Applications.Chat
{
	public class GuildModel
	{
		public string GuildName { get; set; }
		public ServerMember[] Members { get; set; }
		public GuildChannelModel[] Channels { get; set; }
	}
}