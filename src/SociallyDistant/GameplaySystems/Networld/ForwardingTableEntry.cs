namespace SociallyDistant.GameplaySystems.Networld
{
	public class ForwardingTableEntry
	{
		private List<PortForwardingRule> forwardingTable;
		private PortForwardingRule forwardingRule;
		private Dictionary<(uint, ushort), ushort> reservations;

		internal ForwardingTableEntry(List<PortForwardingRule> forwardingTable, PortForwardingRule rule, Dictionary<(uint, ushort), ushort> reservations)
		{
			this.forwardingTable = forwardingTable;
			this.forwardingRule = rule;
			this.reservations = reservations;
		}

		public void Delete()
		{
			this.forwardingTable.Remove(forwardingRule);
			this.reservations.Remove((forwardingRule.InsideAddress, forwardingRule.InsidePort));
		}
	}
}