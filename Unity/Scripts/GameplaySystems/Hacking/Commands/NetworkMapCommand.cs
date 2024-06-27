#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture;
using Core.Systems;
using OS.Network;

namespace GameplaySystems.Hacking.Commands
{
	public class NetworkMapCommand : CommandScript
	{
		/// <inheritdoc />
		protected override async Task OnMain()
		{
			if (!Network.Connected)
			{
				Console.WriteLine("No network connection");
				this.EndProcess();
				return;
			}
			
			// figure out if we should scan a host for ports or scan the network for nearby hosts
			string? hostToScan = null;
			if (this.Arguments.Length >= 1)
				hostToScan = Arguments.First();

			IEnumerable<NetworkInterfaceInformation>? interfaceInfo = this.Network.GetInterfaceInformation();
			if (string.IsNullOrWhiteSpace(hostToScan))
			{
				await ScanHosts(interfaceInfo);
			}
			else if (NetUtility.TryParseCidrNotation(hostToScan, out Subnet subnetToScan))
			{
				await ScanSubnet(subnetToScan);
			}
			else
			{
				await ScanPorts(interfaceInfo, hostToScan);
			}

			Console.ResetWindowTitle();
			this.EndProcess();
		}

		private async Task ScanSubnet(Subnet subnet)
		{
			Console.SetWindowTitle($"Scanning {subnet.CidrNotation} - nmap");
			
			Console.WriteLine("Scanning subnet: " + subnet.CidrNotation);
			Console.WriteLine();
			
			uint mask = subnet.mask;

			uint networkAddress = subnet.networkAddress;

			uint lowerAddress = networkAddress + 1;
			uint upperAddress = ((networkAddress & mask) | ~mask) - 2;

			uint range = upperAddress - lowerAddress;

			var counter = new Counter();

			var hostList = new List<string>();
			
			await Task.Run(async () =>
			{
				for (uint i = 0; i < range; i++)
				{
					uint address = lowerAddress + i;
					await PingAddress(address, counter);
					
					hostList.Clear();
				}
			});

			Console.WriteLine();
			Console.WriteLine($"Scan found {counter.Value} host(s).");
		}

		private async Task PingAddress(uint address, Counter counter)
		{
			await Task.Yield();
			
			PingResult result = await Network.Ping(address, 2, true);

			if (result != PingResult.Pong)
				return;
			
			Console.WriteLine(Network.GetHostName(address));
			counter.CountUp();
		}
		
		private async Task ScanHosts(IEnumerable<NetworkInterfaceInformation>? interfaceInfo)
		{
			foreach (NetworkInterfaceInformation networkInterface in interfaceInfo)
			{
				await ScanHosts(networkInterface);
			}
		}

		private async Task ScanHosts(NetworkInterfaceInformation networkInterface)
		{
			Subnet subnet = networkInterface.ToSubnet();
			
			if (subnet.networkAddress == (NetUtility.LoopbackAddress & subnet.mask))
				return;

			await ScanSubnet(subnet);
		}

		private async Task ScanPorts(IEnumerable<NetworkInterfaceInformation>? interfaceInfo, string host)
		{
			if (!Network.Resolve(host, out uint address))
			{
				Console.WriteLine($"Failed to resolve host {host}.");
				return;
			}
			
			Console.WriteLine($"Starting port scan on host {host} ({NetUtility.GetNetworkAddressString(address)})...");

			await Task.Delay(1420);
			
			Console.WriteLine();

			for (ushort port = 0; port <= 1000; port++)
			{
				PortScanResult result = await Network.ScanPort(address, port);

				if (result.Status == PortStatus.Closed)
					continue;
				
				Console.WriteLine($"{result.Port}\t\t{result.Status}\t\t{result.ServerType}");
				await Task.Delay(175);
			}
			
			Console.WriteLine();
		}
	}
}