using System;
using System.Collections.Generic;
using OS.Devices;
using OS.Network;

namespace GameplaySystems.Networld
{
	public sealed class ListenerHandle
	{
		private ServerInfo serverInfo;
		private DeviceNode? deviceNode;
		private Dictionary<ushort, Listener>? listeners;
		private ushort port;

		public ServerInfo ServerInfo => serverInfo;
		
		public event Action<PacketEvent> PacketReceived; 

		public bool IsValid => deviceNode != null
		                       && listeners != null
		                       && listeners.ContainsKey(port);

		public ListenerHandle(ushort port, Dictionary<ushort, Listener> listeners, DeviceNode node,  ServerType serverType, SecurityLevel secLevel)
		{
			this.deviceNode = node;
			this.listeners = listeners;
			this.port = port;

			if (this.deviceNode != null)
				this.deviceNode.UnhandledPacketReceived += OnPacketReceived;

			ISystemProcess? serverProc = null;
			//if (serverType != ServerType.Netcat)
			//	serverProc = await node.Computer.CreateDaemonProcess(serverType.ToString());
			
			this.serverInfo = new ServerInfo(node.Computer, serverType, secLevel, serverProc);
		}

		private void OnPacketReceived(PacketEvent packetEvent)
		{
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

			if (deviceNode != null)
				deviceNode.UnhandledPacketReceived -= OnPacketReceived;
			
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