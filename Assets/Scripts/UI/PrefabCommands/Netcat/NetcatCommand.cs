#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Architecture;
using GameplaySystems.Networld;
using OS.Network;
using Utility;

namespace UI.PrefabCommands.Netcat
{
	public class NetcatCommand : CommandScript
	{
		/// <inheritdoc />
		protected override async Task OnMain()
		{
			// We don't have an options parser yet so this is a hack
			string usage = "Usage: nc <host> <port>" + Environment.NewLine +
			               "       nc -l <port>";

			if (Arguments.Length != 2)
			{
				Console.WriteLine(usage);
				EndProcess();
				return;
			}
			
			// Second arg must be a ushort
			if (!ushort.TryParse(Arguments[1], out ushort port))
			{
				Console.WriteLine(usage);
				EndProcess();
				return;	
			}
			
			// First must be either "-l" for listen or a valid IP address
			bool isNetworkAddress = NetUtility.TryParseNetworkAddress(Arguments[0], out uint address);
			if (Arguments[0] != "-l" && !isNetworkAddress)
			{
				Console.WriteLine(usage);
				EndProcess();
				return;
			}

			if (Network == null)
			{
				Console.WriteLine("No network connection");
				EndProcess();
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
	}
}