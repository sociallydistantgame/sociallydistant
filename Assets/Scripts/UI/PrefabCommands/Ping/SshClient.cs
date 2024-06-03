#nullable enable
using System;
using System.Threading.Tasks;
using Architecture;
using Core;
using OS.Network;

namespace UI.PrefabCommands.Ping
{
	public class SshClient : CommandScript
	{
		/// <inheritdoc />
		protected override async Task OnMain()
		{
			if (Arguments.Length < 1)
			{
				Console.WriteLine("ssh: usage: ssh <destination> [command [arguments..]]");
				EndProcess();
				return;
			}

			if (Network == null)
			{
				Console.WriteLine("No network connection");
				EndProcess();
				return;
			}
			
			string destination = Arguments[0];
			
			if (!Uri.TryCreate(WithScheme(destination), UriKind.Absolute, out Uri uri))
			{
				Console.WriteLine("fuck");
				EndProcess();
				return;
			}

			string username = uri.UserInfo;
			if (string.IsNullOrWhiteSpace(username))
				username = this.User.UserName;

			ushort port = uri.IsDefaultPort 
				? (ushort) 22 
				: (ushort) uri.Port;

			if (!Network.Resolve(uri.Host, out uint address))
			{
				Console.WriteLine($"ssh: Could not resolve hostname {uri.Host}: Name or service not known");
				EndProcess();
				return;
			}

			ConnectionResult result = await Network.Connect(address, port);

			if (result.Result != ConnectionResultType.Connected)
			{
				string error = SociallyDistantUtility.GetFriendlyNetError(result.Result);
				Console.WriteLine($"ssh: connect to host {uri.Host} port {port}: {error}");
				EndProcess();
				return;
			}

			await Authenticate(result.Connection!, username);
			
			EndProcess();
		}

		private async Task Authenticate(IConnection connection, string username)
		{
			await using var stream = new SimulatedNetworkStream(connection);
			
			
		}
		
		private string WithScheme(string raw)
		{
			if (!raw.StartsWith("ssh://"))
				return $"ssh://{raw}";

			return raw;
		}
	}
}