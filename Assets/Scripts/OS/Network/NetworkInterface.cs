#nullable enable
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Core.Serialization.Binary;
using OS.Network.MessageTransport;
using Utility;
using Random = UnityEngine.Random;

namespace OS.Network
{
	public class NetworkInterface : 
		ISimulationNetworkPort<NetworkInterface>
	{
		private Subnet subnet;
		private uint address;
		private NetworkInterface? otherInterface;
		private Wire<Packet>? cable;
		private WireTerminal<Packet>? ourTerminal;
		private bool addressable = false;

		public bool Connected => otherInterface != null;
		
		public bool Addressable => addressable;

		public string Name { get; }
		public uint NetworkAddress => address;
		public uint SubnetMask => subnet.mask;

		public long MacAddress { get; }
		
		public NetworkInterface(string name = "eth0")
		{
			this.Name = name;
			this.MacAddress = (long) Random.Range(0, long.MaxValue >> 16);
		}

		public void MakeUnaddressable()
		{
			this.addressable = false;
			this.address = default;
			this.subnet = default;
		}
		
		public void MakeAddressable(Subnet subnet, uint address)
		{
			this.subnet = subnet;
			this.address = (subnet.networkAddress & subnet.mask) | (address & ~subnet.mask);
			this.addressable = true;
		}

		public void Connect(NetworkInterface otherInterface)
		{
			if (cable != null)
				throw new InvalidOperationException("This NetworkInterface cannot connect to the specified interface because this interface is already connected to something else. Please disconnect it first. Alternatively, try shoving an Ethernet cable into a port that already has an Ethernet cable plugged into it and see how that goes.");
			
			if (otherInterface.cable != null)
				throw new InvalidOperationException("This NetworkInterface cannot connect to the specified interface because the other interface is already connected to something else. Please disconnect it first. Alternatively, try shoving an Ethernet cable into a port that already has an Ethernet cable plugged into it and see how that goes.");

			// Note: We don't ever talk to this interface, until we're disconnecting...and that's only because this is a simulation
			// and this is the only reliable way to notify each side of the connection that the connection no longer exists.
			this.otherInterface = otherInterface;
			this.otherInterface.otherInterface = this; // Because we can do this.
			
			// Create a virtual cable for both interfaces to share.
			this.cable = new Wire<Packet>();
			this.otherInterface.cable = this.cable;
			
			// We get terminal A, they get terminal B.
			this.ourTerminal = this.cable.TerminalA;
			this.otherInterface.ourTerminal = this.cable.TerminalB;
		}
		
		public void Disconnect()
		{
			if (this.otherInterface == null)
				return;
			
			NetworkInterface? other = this.otherInterface;

			this.otherInterface = null;
			this.cable?.Dispose();
			this.cable = null;
			this.ourTerminal = null;

			other?.Disconnect();
		}

		public void Send(Packet packet)
		{
			// if we're not connected, drop it.
			if (this.ourTerminal == null)
				return;

			ourTerminal.Enqueue(packet);
		}

		public Packet? Receive()
		{
			if (ourTerminal == null)
				return null;
			
			if (ourTerminal.Count == 0)
				return null;

			if (!ourTerminal.TryDequeue(out Packet packet))
				return null;

			return packet;
		}

		public NetworkInterfaceInformation GetInterfaceInformation()
		{
			return new NetworkInterfaceInformation
			{
				Name = this.Name,
				LocalAddress = NetUtility.GetNetworkAddressString(this.NetworkAddress),
				MacAddress = NetUtility.GetMacAddressString(this.MacAddress),
				SubnetMask = NetUtility.GetNetworkAddressString(this.SubnetMask)
			};
		}
	}

	public sealed class LoopbackInterface : ISimulationNetworkInterface
	{
		private readonly ConcurrentQueue<Packet> packets = new ConcurrentQueue<Packet>();
		
		/// <inheritdoc />
		public bool Connected => true;

		/// <inheritdoc />
		public bool Addressable => true;

		/// <inheritdoc />
		public string Name => "lo";

		/// <inheritdoc />
		public uint NetworkAddress => NetUtility.LoopbackAddress;

		/// <inheritdoc />
		public uint SubnetMask => 0xff000000;

		/// <inheritdoc />
		public long MacAddress => long.MaxValue;

		/// <inheritdoc />
		public void Send(Packet packet)
		{
			packets.Enqueue(packet);
		}

		/// <inheritdoc />
		public Packet? Receive()
		{
			if (packets.TryDequeue(out Packet packet))
				return packet;

			return null;
		}

		/// <inheritdoc />
		public NetworkInterfaceInformation GetInterfaceInformation()
		{
			return new NetworkInterfaceInformation()
			{
				Name = Name,
				MacAddress = string.Empty,
				LocalAddress = NetUtility.GetNetworkAddressString(NetworkAddress),
				SubnetMask = NetUtility.GetNetworkAddressString(this.SubnetMask)
			};
		}

		/// <inheritdoc />
		public void MakeUnaddressable()
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public void MakeAddressable(Subnet subnet, uint address)
		{
			throw new NotSupportedException();
		}
	}
}