using System;
using System.Collections.Generic;
using OS.Network;

namespace GameplaySystems.Networld
{
	public sealed class ListenerHandle
	{
		private DeviceNode? deviceNode;
		private Dictionary<ushort, Listener>? listeners;
		private ushort port;

		public event Action<PacketEvent> PacketReceived; 

		public bool IsValid => deviceNode != null
		                       && listeners != null
		                       && listeners.ContainsKey(port);

		public ListenerHandle(ushort port, Dictionary<ushort, Listener> listeners, DeviceNode node)
		{
			this.deviceNode = node;
			this.listeners = listeners;
			this.port = port;

			if (this.deviceNode != null)
				this.deviceNode.UnhandledPacketReceived += OnPacketReceived;
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
			
			deviceNode?.EnqueuePacketForDelivery(packet);
		}
	}
}