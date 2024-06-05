#nullable enable
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Architecture;
using Core;
using Core.Serialization.Binary;
using NetworkServices.Ssh;
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
			using var writer = new BinaryDataWriter(new BinaryWriter(stream, Encoding.UTF8, true));
			using var reader = new BinaryDataReader(new BinaryReader(stream, Encoding.UTF8, true));
			var messageToWrite = new SshMessage();
			var messageRead = new SshMessage();

			messageToWrite.Type = SshPacketType.Username;
			messageToWrite.Data = Encoding.UTF8.GetBytes(username);
			messageToWrite.Write(writer);

			try
			{
				await Task.Run(() => { messageRead.Read(reader); });

				if (messageRead.Type != SshPacketType.Username)
					return;

				do
				{
					Console.Write($"{username}'s password: ");
					string password = await Console.ReadLineAsync();

					messageToWrite.Type = SshPacketType.Password;
					messageToWrite.Data = Encoding.UTF8.GetBytes(password);
					messageToWrite.Write(writer);

					await Task.Run(() => { messageRead.Read(reader); });
				} while (messageRead.Type == SshPacketType.PasswordResult && messageRead.Data?.Length != 0);
			}
			catch (IOException)
			{
				Console.WriteLine($"Connection closed by remote host.");
			}
		}

		private string WithScheme(string raw)
		{
			if (!raw.StartsWith("ssh://"))
				return $"ssh://{raw}";

			return raw;
		}
	}
}