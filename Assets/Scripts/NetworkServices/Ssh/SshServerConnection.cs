#nullable enable
using System;
using System.IO;
using System.Text;
using Core.Serialization;
using Core.Serialization.Binary;
using OS.Network;
using System.Threading.Tasks;
using Core.Scripting;
using OS.Devices;
using OS.Network.MessageTransport;

namespace NetworkServices.Ssh
{
	public sealed class SshServerConnection : IDisposable
	{
		private readonly SshServer server;
		private readonly SimulatedNetworkStream stream;
		private readonly Task thread;
		private readonly WorkQueue workQueue = new();

		public bool IsDone => thread.IsCompleted;
		
		

		public SshServerConnection(SshServer server, IConnection connection)
		{
			this.server = server;
			stream = new SimulatedNetworkStream(connection);
			
			thread = Task.Run(ThreadUpdate);
		}

		public void Update()
		{
			if (IsDone)
			{
				Dispose();
				return;
			}

			workQueue.RunPendingWork();
		}

		private async Task ThreadUpdate()
		{
			using var writer = new BinaryDataWriter(new BinaryWriter(stream, Encoding.UTF8, true));
			using var reader = new BinaryDataReader(new BinaryReader(stream, Encoding.UTF8, true));

			
			var state = State.Username;
			var willFail = false;
			var attemptsLeft = 4;
			IUser? desiredUsername = null;

			while (state != State.Done)
			{
				if (state == State.Running)
				{
					if (desiredUsername == null)
					{
						state = State.Done;
						continue;
					}

					await RunShell(desiredUsername, reader, writer);
					state = State.Done;
				}
				
				var message = new SshMessage();
				
				try
				{
					message.Read(reader);
				}
				catch (IOException)
				{
					// Server shut down by the game
					state = State.Done;
					break;
				}

				switch (state)
				{
					case State.Username:
					{
						if (message.Type != SshPacketType.Username)
						{
							state = State.Done;
							break;
						}

						string user = Encoding.UTF8.GetString(message.Data!);

						willFail = !server.Computer.FindUserByName(user, out desiredUsername);
						message.Write(writer);
						
						state = State.KeyAuth;
						break;
					}
					case State.KeyAuth:
					{
						if (message.Type == SshPacketType.KeyChallenge)
						{
							if (desiredUsername == null || willFail)
							{
								message.Type = SshPacketType.KeyChallengeResult;
								message.Data = Encoding.UTF8.GetBytes("Permission denied (publickey)");
								message.Write(writer);
								state = State.PasswordAuth;
								goto case State.PasswordAuth;
							}
						}
						else if (message.Type == SshPacketType.Password)
						{
							state = State.PasswordAuth;
							goto case State.PasswordAuth;
						}
						else
						{
							message.Type = SshPacketType.Disconnect;
							message.Data = null!;
							
							message.Write(writer);
							state = State.Done;
							break;
						}

						break;
					}
					case State.PasswordAuth:
					{
						if (message.Type != SshPacketType.Password)
						{
							message.Type = SshPacketType.Disconnect;
							message.Data = null!;

							message.Write(writer);
							state = State.Done;
							break;
						}

						string password = Encoding.UTF8.GetString(message.Data!);

						bool success = !willFail && (desiredUsername?.CheckPassword(password) == true);

						await Task.Delay(2000);

						message.Type = SshPacketType.PasswordResult;
						
						if (success)
						{
							message.Data = null!;
							state = State.Running;
						}
						else
						{
							attemptsLeft--;

							if (attemptsLeft == 0)
							{
								message.Type = SshPacketType.Disconnect;
								message.Data = null!;
								state = State.Done;
							}
							else
							{
								message.Data = Encoding.UTF8.GetBytes("Sorry, try again");
							}
						}

						message.Write(writer);
						
						break;
					}
					case State.Running:
					{
						if (desiredUsername == null)
						{
							message.Type = SshPacketType.Disconnect;
							message.Data = null!;
							
							message.Write(writer);

							state = State.Done;
							break;
						}
						
						break;
					}
				}
			}
		}

		private async Task RunShell(IUser user, IDataReader reader, IDataWriter writer)
		{
			var sshConsole = new SshConsole(writer);

			await Task.WhenAll(
				Task.Run(async () => { await ReadKeystrokes(sshConsole, reader); }),
				workQueue.EnqueueAsync(async () => { await StartShell(user, sshConsole); })
			);
		}

		private async Task ReadKeystrokes(SshConsole console, IDataReader reader)
		{
			while (console.Open)
			{
				var message = new SshMessage();
				message.Read(reader);

				switch (message.Type)
				{
					case SshPacketType.Text:
					{
						if (message.Data != null)
						{
							console.ProcessInput(message.Data);
						}
						break;
					}
					case SshPacketType.Disconnect:
					{
						console.Close();
						break;
					}
				}
			}
		}
		
		private async Task StartShell(IUser user, SshConsole console)
		{
			ISystemProcess process = await server.Fork(user);

			var context = new OperatingSystemExecutionContext(process);
			var shell = new InteractiveShell(context);
			
			shell.Setup(console);

			while (console.Open)
			{
				try
				{
					await shell.Run();
				}
				catch (ScriptEndException endException)
				{
					process.Kill(endException.ExitCode);
					console.Close();
				}
			}
		}
		
		/// <inheritdoc />
		public void Dispose()
		{
			stream?.Dispose();
		}

		private enum State
		{
			Username,
			KeyAuth,
			PasswordAuth,
			Running,
			Done
		}
	}

	public enum SshPacketType : byte
	{
		Disconnect,
		Username,
		KeyChallenge,
		KeyChallengeResult,
		Password,
		PasswordResult,
		Text
	}
	
	public struct SshMessage : IPacketMessage
	{
		public SshPacketType Type;
		public byte[] Data;
		
		/// <inheritdoc />
		public void Write(IDataWriter writer)
		{
			writer.Write((byte) Type);

			int count = Data == null ? 0 : Data.Length;
			writer.Write(count);
			
			for (var i = 0; i < count; i++)
				writer.Write(Data[i]);
		}

		/// <inheritdoc />
		public void Read(IDataReader reader)
		{
			Type = (SshPacketType) reader.Read_byte();

			int count = reader.Read_int();

			this.Data = new byte[count];

			for (var i = 0; i < Data.Length; i++)
			{
				Data[i] = reader.Read_byte();
			}
		}
	}
}