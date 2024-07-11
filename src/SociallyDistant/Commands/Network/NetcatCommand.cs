#nullable enable

using System.Text;
using SociallyDistant.Architecture;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Network;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Commands.Network
{
	[Command("nc")]
	public class NetcatCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override async Task OnExecute()
		{
			// We don't have an options parser yet so this is a hack
			string usage = "Usage: nc <host> <port>" + Environment.NewLine +
			               "       nc -l <port>";

			if (Arguments.Length != 2)
			{
				Console.WriteLine(usage);
				return;
			}
			
			// Second arg must be a ushort
			if (!ushort.TryParse(Arguments[1], out ushort port))
			{
				Console.WriteLine(usage);
				return;	
			}
			
			// First must be either "-l" for listen or a valid IP address
			bool isNetworkAddress = NetUtility.TryParseNetworkAddress(Arguments[0], out uint address);
			if (Arguments[0] != "-l" && !isNetworkAddress)
			{
				Console.WriteLine(usage);
				return;
			}

			if (Network == null)
			{
				Console.WriteLine("No network connection");
				return;
			}

			if (isNetworkAddress)
			{
				ConnectionResult result = await Network.Connect(address, port);
				await ClientCallback(result);
			}
			else
			{
				// We're a listener
				await WaitForConnection(Network.Listen(port));
			}
		}

		private async Task ClientCallback(ConnectionResult result)
		{
			if (result.Result == ConnectionResultType.Connected && result.Connection != null)
				await ConnectionLoop(result.Connection);
		}

		private async Task ServerCallback(IConnection connection)
		{
			await ConnectionLoop(connection);
		}

		private async Task ConnectionLoop(IConnection connection)
		{
			while (connection.Connected)
			{
				while (connection.Receive(out byte[] data))
				{
					Console.WriteLine(Encoding.UTF8.GetString(data));
				}

				while (Console.ReadLine(out string text))
				{
					connection.Send(Encoding.UTF8.GetBytes(text));
				}

				await Task.Yield();
			}
		}
		
		private async Task WaitForConnection(IListener listener)
		{
			IConnection? connection = null;
			while ((connection = listener.AcceptConnection()) == null)
				await Task.Yield();

			ServerCallback(connection);
		}

		public NetcatCommand(IGameContext gameContext) : base(gameContext)
		{
		}
	}
}