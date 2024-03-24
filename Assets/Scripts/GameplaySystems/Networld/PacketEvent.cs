using System.Collections.Concurrent;
using System.Collections.Generic;
using OS.Network;

namespace GameplaySystems.Networld
{
	public sealed class PacketEvent
	{
		private readonly DeviceNode deviceNode;
		private readonly Packet packet;
		private bool refused;
		private bool handled;

		public bool Refused => refused;
		public bool Handled => handled;

		public void Refuse()
		{
			refused = true;
			handled = true;
		}

		public Packet Packet => packet;

		public PacketEvent(Packet packet, DeviceNode deviceNode)
		{
			this.packet = packet;
			this.deviceNode = deviceNode;
		}

		public void Handle(Packet responsePacket)
		{
			if (handled)
				return;
			
			handled = true;
			this.deviceNode.EnqueuePacketForDelivery(responsePacket);
		}

		public void Handle()
		{
			handled = true;
		}
	}
}