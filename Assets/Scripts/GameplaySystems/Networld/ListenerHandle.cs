using System;
using System.Collections.Generic;
using OS.Devices;
using OS.Network;

namespace GameplaySystems.Networld
{
	public sealed class ListenerHandle
	{
		private readonly IPacketQueue queue;
		private ServerInfo serverInfo;
		private DeviceNode? deviceNode;
		private Dictionary<ushort, Listener>? listeners;
		private ushort port;

		public ServerInfo ServerInfo => serverInfo;
		
		public event Action<PacketEvent> PacketReceived; 

		public bool IsValid => deviceNode != null
		                       && listeners != null
		                       && listeners.ContainsKey(port);

		public ListenerHandle(ushort port, Dictionary<ushort, Listener> listeners, DeviceNode node, IPacketQueue queue,  ServerType serverType, SecurityLevel secLevel)
		{
			this.deviceNode = node;
			this.listeners = listeners;
			this.port = port;
			this.queue = queue;
			
			this.queue.Received += OnPacketReceived;

			ISystemProcess? serverProc = null;
			//if (serverType != ServerType.Netcat)
			//	serverProc = await node.Computer.CreateDaemonProcess(serverType.ToString());
			
			this.serverInfo = new ServerInfo(node.Computer, serverType, secLevel, serverProc);
		}

		private void OnPacketReceived(Packet packet)
		{
			var packetEvent = new PacketEvent(packet, deviceNode);
			
			if (packetEvent.Handled)
				return;

			if (packetEvent.Packet.DestinationPort != port)
				return;

			PacketReceived?.Invoke(packetEvent);
		}

		public void Invalidate()
		{
			if (!IsValid)
				return;
			
			queue.Received -= OnPacketReceived;

			queue.Dispose();
			listeners?.Remove(port);
			listeners = null;
			deviceNode = null;
		}

		public void Send(Packet packet)
		{
			if (!IsValid)
				return;

			packet.SourcePort = this.port;
			deviceNode?.EnqueuePacketForDelivery(packet);
		}
	}
}