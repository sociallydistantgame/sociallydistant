#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OS.Devices;
using OS.Network;

namespace NetworkServices.Ssh
{
	[UsedImplicitly]
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