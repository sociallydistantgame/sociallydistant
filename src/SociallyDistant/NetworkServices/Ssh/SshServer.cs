#nullable enable

using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.Network;

namespace SociallyDistant.NetworkServices.Ssh
{
	[NetworkService("ssh")]
	public sealed class SshServer : 
		NetworkService,
		IUserSpecifiedPort
	{
		private readonly List<SshServerConnection> activeConnections = new();
		private IListener? listener;
		
		/// <inheritdoc />
		public SshServer(ISystemProcess process) : base(process)
		{
			process.Name = "sshd";
		}

		/// <inheritdoc />
		public override void Start()
		{
			listener = Network?.Listen(Port);
		}

		/// <inheritdoc />
		public override void Update()
		{
			if (listener == null)
				return;

			IConnection? nextConnection = listener.AcceptConnection();
			if (nextConnection != null)
				AcceptClient(nextConnection);

			for (var i = 0; i < activeConnections.Count; i++)
			{
				activeConnections[i].Update();

				if (this.activeConnections[i].IsDone)
					this.activeConnections.RemoveAt(i);
			}
		}

		private void AcceptClient(IConnection connection)
		{
			var serverConnection = new SshServerConnection(this, connection);

			this.activeConnections.Add(serverConnection);
		}

		/// <inheritdoc />
		public override void Stop()
		{
			while (activeConnections.Count > 0)
			{
				activeConnections[0].Dispose();
				activeConnections.RemoveAt(0);
			}
			
			listener?.Close();
			listener = null;
		}

		/// <inheritdoc />
		public ushort Port { get; set; }
	}
}