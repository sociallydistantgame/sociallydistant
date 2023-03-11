using System;

namespace GameplaySystems.Networld
{
	public class LocalAreaNetwork
	{
		private Edge<LocalAreaNetwork, InternetServiceProvider> connection = new Edge<LocalAreaNetwork, InternetServiceProvider>();
		private NetworkSimulationController simulation;
		private LocalAreaNode node;
		
		public Guid Token => connection.Token;

		public InternetServiceProvider? InternetServiceProvider => connection.Side2;

		public LocalAreaNetwork(NetworkSimulationController simulation, LocalAreaNode node)
		{
			this.simulation = simulation;
			this.node = node;
			this.connection.Side1 = this;
		}
		
		public void ConnectToInternet(InternetServiceProvider isp)
		{
			DisconnectFromInternet();

			isp.ConnectLan(this.connection);
		}

		public void DisconnectFromInternet()
		{
			if (connection.Side2 == null)
				return;

			connection.Side2.DisconnectLan(this);
		}

		public NetworkConnection CreateDevice()
		{
			return this.node.SetUpNewDevice();
		}

		public void DeleteDevice(NetworkConnection connection)
		{
			this.node.DeleteDevice(connection);
		}

		public ForwardingTableEntry GetForwardingRule(NetworkConnection connection, ushort insidePort, ushort outsidePort)
		{
			return this.node.GetForwardingRule(connection, insidePort, outsidePort);
		}

		public bool ContainsDevice(NetworkConnection connection)
		{
			return node.ContainsDevice(connection);
		}
	}
}