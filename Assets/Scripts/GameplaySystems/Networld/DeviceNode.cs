using OS.Network;
using Utility;

namespace GameplaySystems.Networld
{
	public class DeviceNode : 
		IDeviceWithNetworkInterface,
		INetworkNode
	{
		private NetworkInterface deviceInterface = new NetworkInterface();
		private NetworkInterface loopbackInterface;
		private readonly NetworkConnection connection;
		private Subnet localSubnet;
		private uint defaultGateway;
		private uint localAddress;
		private NetworkInterface loopbackOutput;

		public NetworkInterface LoopbackInterface => loopbackInterface;
		public uint DefaultGateway => defaultGateway;
		
		public NetworkConnection NetworkConnection => connection;
		
		public DeviceNode(Subnet localSubnet, uint defaultGateway, uint localAddress)
		{
			this.localSubnet = localSubnet;
			this.defaultGateway = defaultGateway;
			this.localAddress = localAddress;
			
			// Make the interface addressable
			this.deviceInterface.MakeAddressable(localSubnet, localAddress);
			
			// Create a loopback interface that...loops back to us.
			this.loopbackInterface = new NetworkInterface("lo");
			this.loopbackInterface.MakeAddressable(NetUtility.LoopbackSubnet, NetUtility.LoopbackAddress);

			// The loopback output interface is how we actually read what's sent to 127.0.0.1.
			// It's confusing as fuck but essentially:
			// - Game exposes the 127.0.0.1 iface we created above
			// - Game sends packet through that interface
			// - We read that packet through loopback output
			// - We send packet to the relevant Listener based solely on the port.
			this.loopbackOutput = new NetworkInterface("lo");
			this.loopbackOutput.Connect(this.loopbackInterface);
			
			// Create the NetworkConnection that controls us.
			var newConnection = new NetworkConnection(this);
			this.connection = newConnection;
		}
		
		/// <inheritdoc />
		public void NetworkUpdate()
		{
			
		}

		/// <inheritdoc />
		public NetworkInterface NetworkInterface => deviceInterface;
	}
}