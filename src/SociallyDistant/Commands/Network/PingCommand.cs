#nullable enable
using SociallyDistant.Architecture;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Network;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Commands.Network
{
	[Command("ping")]
	public class PingCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override async Task OnExecute()
		{
			if (Network == null)
			{
				Console.WriteLine("No network connection");
				return;
			}

			if (Arguments.Length != 1)
			{
				Console.WriteLine("ping: Usage: ping <host>");
				return;
			}

			string host = Arguments[0];
			if (!Network.Resolve(host, out uint address))
			{
				Console.WriteLine($"ping: could not resolve host {host}");
				return;
			}

			await Ping(address, 32);
		}

		private async Task Ping(uint address, int amountToPing)
		{
			for (var i = 0; i < amountToPing; i++)
			{
				if (Network == null)
				{
					Console.WriteLine("No network connection");
					return;
				}

				PingResult result = await Network.Ping(address, 4, false);

				switch (result)
				{
					case PingResult.TimedOut:
						Console.WriteLine("Request timed out");
						break;
					case PingResult.Pong:
						Console.WriteLine($"Reply from {NetUtility.GetNetworkAddressString(address)}");
						await Task.Delay(100);
						break;
				}
			}
		}

		public PingCommand(IGameContext gameContext) : base(gameContext)
		{
		}
	}
}