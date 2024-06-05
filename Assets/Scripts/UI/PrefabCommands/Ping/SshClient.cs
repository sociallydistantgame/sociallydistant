#nullable enable
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Architecture;
using Core;
using Core.Serialization;
using Core.Serialization.Binary;
using NetworkServices.Ssh;
using OS.Devices;
using OS.Network;
using OS.Network.MessageTransport;
using UnityEngine;

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

			await using (var stream = new SimulatedNetworkStream(result.Connection!))
			{
				bool authenticated = await Authenticate(stream, username);
				if (!authenticated)
					return;

				var workQueue = new WorkQueue();

				Task keyStrokeTask = SendKeys(stream, workQueue);
				
				await Task.WhenAll(
					Task.Run(async () =>
					{
						await ReadSshOutput(stream, workQueue);
					}),
					keyStrokeTask
				);
			}

			EndProcess();
		}

		private async Task ReadSshOutput(SimulatedNetworkStream stream, WorkQueue workQueue)
		{
			using var reader = new BinaryDataReader(new BinaryReader(stream, Encoding.UTF8, true));
			
			while (stream.Connected)
			{
				var message = new SshMessage();
				message.Read(reader);

				if (message.Type == SshPacketType.Text)
				{
					string text = Encoding.UTF8.GetString(message.Data!);

					await workQueue.EnqueueAsync(() =>
					{
						Console.Write(text);
					});
				}
				else if (message.Type == SshPacketType.Disconnect)
				{
					// This tells the other task to cancel
					workQueue.Enqueue(() => throw new OperationCanceledException());
					break;
				}
			}
		}
		
		private async Task SendKeys(SimulatedNetworkStream stream, WorkQueue workQueue)
		{
			using var writer = new BinaryDataWriter(new BinaryWriter(stream, Encoding.UTF8, true));
			while (stream.Connected)
			{
				try
				{
					workQueue.RunPendingWork();
				}
				catch (OperationCanceledException)
				{
					// We were told to exit because the server sent a Disconnect request.
					break;
				}

				ConsoleInputData? keyTry = Console.ReadKey();
				if (keyTry == null)
				{
					await Task.Yield();
					continue;
				}

				ConsoleInputData key = keyTry.Value;
				
				var message = new SshMessage();
				message.Type = SshPacketType.Text;
				if (key.KeyCode == KeyCode.None)
				{
					message.Data = Encoding.UTF8.GetBytes(key.Character.ToString());
				}
				else
				{
					// We need to encode the key as a Socially Distant Keystroke escape sequence.
					// That's all we need to send to the remote server to send keyboard events.
					// See https://man.sociallydistantgame.com/index.php/Terminal_Escape_Sequences#Keystroke_events

					var code = (int) key.KeyCode;
					var modifiers = (int) key.Modifiers;

					message.Data = Encoding.UTF8.GetBytes($"\x1b[{code};{modifiers}~");
				}

				message.Write(writer);
			}
		}
		
		private async Task<bool> Authenticate(SimulatedNetworkStream stream, string username)
		{
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
					return false;

				do
				{
					Console.Write($"{username}'s password: ");
					string password = await Console.ReadLineAsync();

					messageToWrite.Type = SshPacketType.Password;
					messageToWrite.Data = Encoding.UTF8.GetBytes(password);
					messageToWrite.Write(writer);

					await Task.Run(() => { messageRead.Read(reader); });
				} while (messageRead.Type == SshPacketType.PasswordResult && messageRead.Data?.Length != 0);

				return (messageRead.Type == SshPacketType.PasswordResult && messageRead.Data?.Length == 0);
			}
			catch (IOException)
			{
				Console.WriteLine($"Connection closed by remote host.");
				return false;
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