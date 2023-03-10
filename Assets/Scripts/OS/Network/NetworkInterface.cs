#nullable enable
using System;
using System.IO;
using System.Text;
using Core.MessageTransport;
using Core.Serialization.Binary;
using Utility;
using Random = UnityEngine.Random;

namespace OS.Network
{
	public class NetworkInterface
	{
		private Subnet subnet;
		private uint address;
		private NetworkInterface? otherInterface;
		private Wire<byte>? cable;
		private WireTerminal<byte>? ourTerminal;
		private WireTerminalStream? stream;
		private BinaryReader? bReader;
		private BinaryWriter? bWriter;
		private BinaryDataWriter? writer;
		private BinaryDataReader? reader;
		private bool addressable = false;

		public bool Connected => otherInterface != null;
		
		public bool Addressable => addressable;

		public string Name { get; }
		public uint NetworkAddress => address;
		public uint SubnetMask => subnet.Mask;

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
			if ((address & subnet.Mask) != subnet.NetworkAddress)
				throw new InvalidOperationException("Addressable NetworkInterface address must be in the specified subnet's IP range.");

			this.subnet = subnet;
			this.address = address;
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
			this.cable = new Wire<byte>();
			this.otherInterface.cable = this.cable;
			
			// We get terminal A, they get terminal B.
			this.ourTerminal = this.cable.TerminalA;
			this.otherInterface.ourTerminal = this.cable.TerminalB;
			
			// Set up our comm streams.
			this.CreateStream();
			this.otherInterface.CreateStream();
		}

		private void CreateStream()
		{
			if (ourTerminal == null)
				throw new InvalidOperationException("Cannot create a communication stream between two NetworkInterfaces if one of the interfaces is disconnected!");
			
			this.stream = new WireTerminalStream(ourTerminal);
			this.bReader = new BinaryReader(stream, Encoding.UTF8);
			this.bWriter = new BinaryWriter(stream, Encoding.UTF8);
			this.reader = new BinaryDataReader(bReader);
			this.writer = new BinaryDataWriter(bWriter);
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

			this.bReader?.Dispose();
			this.bWriter?.Dispose();
			this.reader?.Dispose();
			this.writer?.Dispose();
			this.reader = null;
			this.writer = null;
			this.bReader = null;
			this.bWriter = null;

			other?.Disconnect();
		}

		public void Send(Packet packet)
		{
			// if we're not connected, drop it.
			if (this.writer == null)
				return;

			packet.Write(this.writer);
		}

		public Packet? Receive()
		{
			if (reader == null)
				return null;

			if (ourTerminal == null)
				return null;

			if (ourTerminal.Count == 0)
				return null;
			
			var packet = new Packet();
			packet.Read(reader);
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
}