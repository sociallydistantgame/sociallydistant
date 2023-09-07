#nullable enable
using System;
using System.Collections;
using Architecture;
using GameplaySystems.Networld;
using OS.Network;
using Utility;

namespace UI.PrefabCommands.Netcat
{
	public class PingCommand : CommandScript
	{
		private void Start()
		{
			if (Network == null)
			{
				Console.WriteLine("No network connection");
				EndProcess();
				return;
			}

			if (Arguments.Length != 1)
			{
				Console.WriteLine("ping: Usage: ping <host>");
				EndProcess();
				return;
			}

			string host = Arguments[0];
			if (!NetUtility.TryParseNetworkAddress(host, out uint address))
			{
				Console.WriteLine("ping: Usage: ping <host>");
				EndProcess();
				return;
			}

			StartCoroutine(Ping(address, 32));
		}

		private IEnumerator Ping(uint address, int amountToPing)
		{
			for (var i = 0; i < amountToPing; i++)
			{
				if (Network == null)
				{
					Console.WriteLine("No network connection");
					EndProcess();
					yield break;
				}
				
				yield return StartCoroutine(Network.Ping(address, 30, (result) =>
				{
					switch (result)
					{
						case PingResult.TimedOut:
							Console.WriteLine("Request timed out");
							break;
						case PingResult.Pong:
							Console.WriteLine($"Reply from {NetUtility.GetNetworkAddressString(address)}");
							break;
					}
				}));
			}
		}
	}
}