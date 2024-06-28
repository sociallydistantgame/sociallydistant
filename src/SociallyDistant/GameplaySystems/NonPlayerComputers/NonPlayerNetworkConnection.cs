#nullable enable
using SociallyDistant.Core.OS.Network;
using SociallyDistant.GameplaySystems.Networld;
using SociallyDistant.OS.Network;

namespace SociallyDistant.GameplaySystems.NonPlayerComputers
{
	public sealed class NonPlayerNetworkConnection : INetworkConnection
	{
		private readonly Guid id = Guid.NewGuid();
		private readonly NonPlayerComputer device;
		
		private LocalAreaNetwork? currentLan;
		private NetworkConnection? actualConnection;
		private LoopbackInterface? loopback;

		public NonPlayerNetworkConnection(NonPlayerComputer device)
		{
			this.device = device;
		}
        
		public void Connect(LocalAreaNetwork lan)
		{
			if (currentLan == lan)
				return;

			loopback = null;

			currentLan = lan;
			actualConnection = currentLan.CreateDevice(device);
		}

		public void Disconnect()
		{
			if (currentLan == null)
				return;
			
			if (actualConnection != null)
				currentLan.DeleteDevice(actualConnection);

			actualConnection = null;
			currentLan = null;

			loopback = new LoopbackInterface();
		}
		
		/// <inheritdoc />
		public IEnumerable<NetworkInterfaceInformation> GetInterfaceInformation()
		{
			if (loopback != null)
				yield return loopback.GetInterfaceInformation();

			if (actualConnection != null)
			{
				foreach (NetworkInterfaceInformation info in actualConnection.GetInterfaceInformation())
					yield return info;
			}
		}

		/// <inheritdoc />
		public Guid Identifier => actualConnection?.Identifier ?? id;

		/// <inheritdoc />
		public bool Connected => actualConnection?.Connected == true;

		/// <inheritdoc />
		public Task<PingResult> Ping(uint address, float timeout, bool acceptVoidPackets)
		{
			if (actualConnection != null)
			{
				return actualConnection.Ping(address, timeout, acceptVoidPackets);
			}

			if ((address & 0x7f000000) != 0)
			{
				return Task.FromResult(PingResult.Pong);
			}

			return Task.FromResult(PingResult.TimedOut);
		}

		/// <inheritdoc />
		public Task<ConnectionResult> Connect(uint remoteAddress, ushort remotePort)
		{
			if (actualConnection != null)
				return actualConnection.Connect(remoteAddress, remotePort);

			return Task.FromResult(new ConnectionResult
			{
				Result = ConnectionResultType.Refused
			});
		}

		/// <inheritdoc />
		public IListener Listen(ushort port, ServerType serverType = ServerType.Netcat, SecurityLevel secLevel = SecurityLevel.Open)
		{
			return actualConnection?.Listen(port, serverType, secLevel)!;
		}

		/// <inheritdoc />
		public bool Resolve(string host, out uint address)
		{
			if (actualConnection != null)
				return actualConnection.Resolve(host, out address);

			address = 0;

			if (host == "localhost")
			{
				address = 0x7f000001;
				return true;
			}

			if (NetUtility.TryParseNetworkAddress(host, out address))
				return true;

			return false;
		}

		/// <inheritdoc />
		public Task<PortScanResult> ScanPort(uint address, ushort port)
		{
			if (actualConnection != null)
			{
				return actualConnection.ScanPort(address, port);
			}

			return Task.FromResult(new PortScanResult(port, PortStatus.Closed, ServerType.Netcat));
		}

		/// <inheritdoc />
		public bool IsListening(ushort port)
		{
			return actualConnection?.IsListening(port) == true;
		}

		/// <inheritdoc />
		public string GetHostName(uint address)
		{
			if (actualConnection != null)
				return actualConnection.GetHostName(address);

			if (address == 0x7f000001)
				return "localhost";
            
			return NetUtility.GetNetworkAddressString(address);
		}
	}
}