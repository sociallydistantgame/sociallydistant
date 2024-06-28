namespace SociallyDistant.GameplaySystems.Networld
{
	public class PortForwardingRule
	{
		public bool IsForOutgoing { get; set; }
		public uint GlobalAddress { get; set; }
		public uint InsideAddress { get; set; }
		public uint OutsideAddress { get; set; }
		public ushort InsidePort { get; set; }
		public ushort OutsidePort { get; set; }
		public ushort GlobalPort { get; set; }
	}
}