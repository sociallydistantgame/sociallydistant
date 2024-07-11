#nullable enable
using SociallyDistant.Architecture;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Network;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Commands.Network
{
	[Command("ifconfig")]
	public class NetworkInfoCommand : ScriptableCommand
	{
		/// <inheritdoc />
		protected override Task OnExecute()
		{
			Console.WriteLine();
			Console.WriteLine("Network Configuration");
			Console.WriteLine();

			if (this.Network == null)
			{
				Console.WriteLine("  No network interfaces found.");
				Console.WriteLine();
				return Task.CompletedTask;
			}
			
			foreach (NetworkInterfaceInformation iface in this.Network.GetInterfaceInformation())
			{
				Console.WriteLine($@"
  Network interface:      {iface.Name}
    MAC address:          {iface.MacAddress}
    Local IPv4 address:   {iface.LocalAddress}
    Subnet mask:          {iface.SubnetMask}
    Default gateway:      {iface.DefaultGateway}");
			}
			
			return Task.CompletedTask;
		}

		public NetworkInfoCommand(IGameContext gameContext) : base(gameContext)
		{
		}
	}
}