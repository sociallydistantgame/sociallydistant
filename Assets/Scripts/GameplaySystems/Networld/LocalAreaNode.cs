using System;
using System.Collections.Generic;
using OS.Network;

namespace GameplaySystems.Networld
{
	public class LocalAreaNode : INetworkSwitch
	{
		private List<NetworkInterface> insideInterfaces = new List<NetworkInterface>();
		private NetworkInterface outboundInterface = new NetworkInterface();
		private List<DeviceNode> devices = new List<DeviceNode>();
		private Subnet insideNetwork;
		private Dictionary<uint, DeviceNode> reservations = new Dictionary<uint, DeviceNode>();
		private uint simulatedDefaultGatewayAddress;

		public LocalAreaNode(Subnet insideLocalSubnet)
		{
			this.insideNetwork = insideLocalSubnet;
			this.simulatedDefaultGatewayAddress = this.insideNetwork.FirstHost;
			
		}

		/// <inheritdoc />
		public void NetworkUpdate()
		{
			// Update all devices
			for (var i = 0; i < devices.Count; i++)
				devices[i].NetworkUpdate();
		}

		/// <inheritdoc />
		public IEnumerable<NetworkInterface> Neighbours => insideInterfaces;

		/// <inheritdoc />
		public NetworkInterface NetworkInterface => outboundInterface;

		public NetworkConnection SetUpNewDevice()
		{
			// Reserve an IP address for the device
			uint networkAddress = simulatedDefaultGatewayAddress + 1;
			while (reservations.ContainsKey(networkAddress))
				networkAddress++;

			if (networkAddress > insideNetwork.LastHost)
				throw new InvalidOperationException("Maximum number of device hosts reached for this LAN.");
			
			// Create the new device node... pretend this line of code just magically bought a computer.
			// We don't know what computer it is but it has an Ethernet port and it's our job to
			// wire it up.
			var deviceNode = new DeviceNode(insideNetwork, simulatedDefaultGatewayAddress, networkAddress);
			devices.Add(deviceNode);
			reservations.Add(networkAddress, deviceNode);

			// Now pretend we're a network switch that can have infinitely many Ethernet ports on it, but we must
			// ask the universe to create more ports on our chassis before we can use them. This does that.
			var newInterface = new NetworkInterface();
			this.insideInterfaces.Add(newInterface);
			
			// Configure the new interface to have the same IP address and subnet as the device connected to it.
			newInterface.MakeAddressable(this.insideNetwork, networkAddress);
			
			// Now, grab an Ethernet patch cable and connect the device to the network!
			deviceNode.NetworkInterface.Connect(newInterface);
			
			// Return the NetworkConnection that the rest of the game is allowed to use.
			return deviceNode.NetworkConnection;
		}
	}
}