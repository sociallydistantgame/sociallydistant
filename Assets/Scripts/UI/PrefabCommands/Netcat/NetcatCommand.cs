#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Architecture;
using GameplaySystems.Networld;
using Utility;

namespace UI.PrefabCommands.Netcat
{
	public class NetcatCommand : CommandScript
	{
		private void Start()
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
				StartCoroutine(Network.Connect(address, port, ClientCallback));
			}
			else
			{
				// We're a listener
				StartCoroutine(WaitForConnection(Network.Listen(port)));
			}
		}

		private void ClientCallback(ConnectionResult result)
		{
			if (result.Result == ConnectionResultType.Connected && result.Connection != null)
				StartCoroutine(ConnectionLoop(result.Connection));
		}

		private void ServerCallback(Connection connection)
		{
			StartCoroutine(ConnectionLoop(connection));
		}

		private IEnumerator ConnectionLoop(Connection connection)
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
				
				yield return null;
			}
		}
		
		private IEnumerator WaitForConnection(Listener listener)
		{
			Connection? connection = null;
			while ((connection = listener.AcceptConnection()) == null)
				yield return null;

			ServerCallback(connection);
		}
	}
}