using System;
using System.Collections.Generic;
using OS.Network;
using Utility;

namespace GameplaySystems.Networld
{
	public class NetworkConnection : INetworkInterfaceEnumerator
	{
		private DeviceNode deviceNode;

		public string LocalInterfaceName => deviceNode.NetworkInterface.Name;
		public string LoopbackInterfaceName => deviceNode.LoopbackInterface.Name;
		public string LocalAddress => NetUtility.GetNetworkAddressString(deviceNode.NetworkInterface.NetworkAddress);
		public string SubnetMask => NetUtility.GetNetworkAddressString(deviceNode.NetworkInterface.SubnetMask);
		public string LoopbackAddress => NetUtility.GetNetworkAddressString(deviceNode.LoopbackInterface.NetworkAddress);
		
		public NetworkConnection(DeviceNode node)
		{
			if (node.NetworkConnection != null)
				throw new InvalidOperationException("Cannot create a NetworkConnection object outside of the constructor of a DeviceNode.");
			
			this.deviceNode = node;
		}

		/// <inheritdoc />
		public IEnumerable<NetworkInterfaceInformation> GetInterfaceInformation()
		{
			yield return deviceNode.LoopbackInterface.GetInterfaceInformation();
			
			NetworkInterfaceInformation mainInfo = deviceNode.NetworkInterface.GetInterfaceInformation();
			mainInfo.DefaultGateway = NetUtility.GetNetworkAddressString(deviceNode.DefaultGateway);
			yield return mainInfo;
		}
	}
}