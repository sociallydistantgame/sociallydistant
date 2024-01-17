#nullable enable
using Architecture;
using OS.Network;
using UnityEngine;
using System.Threading.Tasks;

namespace UI.ScriptableCommands
{
	[CreateAssetMenu(menuName = "ScriptableObject/Scriptable Commands/Network Info Command")]
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
	}
}