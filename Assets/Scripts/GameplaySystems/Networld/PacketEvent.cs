using System.Collections.Generic;
using OS.Network;

namespace GameplaySystems.Networld
{
	public sealed class PacketEvent
	{
		private readonly Queue<Packet> sendQueue;
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

		public PacketEvent(Packet packet, Queue<Packet> sendQueue)
		{
			this.packet = packet;
			this.sendQueue = sendQueue;
		}

		public void Handle(Packet responsePacket)
		{
			if (handled)
				return;
			
			handled = true;
			this.sendQueue.Enqueue(responsePacket);
		}

		public void Handle()
		{
			handled = true;
		}
	}
}