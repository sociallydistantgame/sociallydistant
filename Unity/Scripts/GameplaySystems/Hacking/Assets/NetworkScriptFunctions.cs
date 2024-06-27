#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Scripting;
using Core.WorldData.Data;
using OS.Network;

namespace GameplaySystems.Hacking.Assets
{
	public sealed class NetworkScriptFunctions : ScriptModule
	{
		private readonly IWorldManager worldManager;
		private readonly string networkId;

		public NetworkScriptFunctions(IWorldManager worldManager, string networkId)
		{
			this.worldManager = worldManager;
			this.networkId = networkId;
		}

		private void UpdateNetwork(Func<WorldLocalNetworkData, WorldLocalNetworkData> updater)
		{
			WorldLocalNetworkData network = worldManager.World.LocalAreaNetworks.GetNarrativeObject(this.networkId);

			network = updater(network);
			
			worldManager.World.LocalAreaNetworks.Modify(network);
		}

		[Function("name")]
		public void SetNetworkName(string[] name)
		{
			UpdateNetwork(n =>
			{
				n.Name = string.Join(" ", name);
				return n;
			});
		}

		[Function("device")]
		public void CreateDevice(string narrativeId, string name)
		{
			var fullNarrativeId = $"{networkId}:{narrativeId}";

			WorldComputerData device = worldManager.World.Computers.GetNarrativeObject(fullNarrativeId);

			device.HostName = name;

			if (device.MacAddress == 0)
				device.MacAddress = NetUtility.GetRandomMacAddress();
			
			worldManager.World.Computers.Modify(device);

			WorldLocalNetworkData network = worldManager.World.LocalAreaNetworks.GetNarrativeObject(networkId);
			WorldNetworkConnection connection = worldManager.World.NetworkConnections.FirstOrDefault(x => x.ComputerId == device.InstanceId);

			ObjectId oldLanId = connection.LanId;
			connection.LanId = network.InstanceId;
			
			if (connection.ComputerId != device.InstanceId)
			{
				connection.InstanceId = worldManager.GetNextObjectId();
				connection.ComputerId = device.InstanceId;
				
				worldManager.World.NetworkConnections.Add(connection);
			}
			else
			{
				if (connection.LanId != oldLanId)
					worldManager.World.NetworkConnections.Modify(connection);
			}
		}
		
		[Function("domain")]
		public void BindDomain(string domainName)
		{
			WorldLocalNetworkData network = worldManager.World.LocalAreaNetworks.GetNarrativeObject(networkId);
			WorldDomainNameData domainEntry = worldManager.World.Domains.FirstOrDefault(x => x.RecordName == domainName);
			WorldInternetServiceProviderData isp = worldManager.World.InternetProviders[network.ServiceProviderId];

			if (!NetUtility.TryParseCidrNotation(isp.CidrNetwork, out Subnet ispSubnet))
				throw new InvalidOperationException("A network must be connected to an Internet Service Provider before a domain name can be assigned.");

			uint networkAddress = (ispSubnet.networkAddress & ispSubnet.mask) | (network.PublicNetworkAddress & ~ispSubnet.mask);
			domainEntry.Address = networkAddress;
			
			if (domainEntry.RecordName != domainName)
			{
				domainEntry.InstanceId = worldManager.GetNextObjectId();
				domainEntry.RecordName = domainName;

				worldManager.World.Domains.Add(domainEntry);
			}
			else
			{
				worldManager.World.Domains.Modify(domainEntry);
			}
		}
		
		[Function("isp")]
		public void SetServiceProvider(string narrativeId)
		{
			UpdateNetwork(n =>
			{
				WorldInternetServiceProviderData isp = worldManager.World.InternetProviders.GetNarrativeObject(narrativeId);
				
				if (string.IsNullOrWhiteSpace(isp.CidrNetwork))
					isp.CidrNetwork = worldManager.GetNextIspRange();

				if (!worldManager.World.InternetProviders.ContainsId(isp.InstanceId))
				{
					worldManager.World.InternetProviders.Add(isp);
				}
				else
				{
					worldManager.World.InternetProviders.Modify(isp);
				}
				
				if (n.ServiceProviderId != isp.InstanceId)
				{
					n.ServiceProviderId = isp.InstanceId;
					n.PublicNetworkAddress = worldManager.GetNextPublicAddress(isp.InstanceId);
				}
				
				return n;
			});
		}

		[Function("service")]
		public void SetupDeviceService(string clause, string serviceId, string deviceId, string rawPort)
		{
			bool enable = clause.ToLower() switch
			{
				"enable" => true,
				"disable" => false,
				_ => throw new InvalidOperationException("service: Argument 1 must either be 'enable' or 'disable'")
			};
			
			ushort port = ushort.Parse(rawPort);
			var fullNarrativeId = $"{networkId}:{deviceId}";

			WorldComputerData device = worldManager.World.Computers.GetNarrativeObject(fullNarrativeId);

			Dictionary<string, NetworkServiceData> services = device.Services.ToDictionary(x => x.Id, x => x);

			services[serviceId] = new NetworkServiceData
			{
				Id = serviceId,
				Enabled = enable,
				Port = port
			};

			device.Services = services.Values.ToArray();

			worldManager.World.Computers.Modify(device);
		}
		
		[Function("forward")]
		private void ForwardPort(string outsideRaw, string deviceId, string insideRaw)
		{
			ushort outside = ushort.Parse(outsideRaw);
			ushort inside = ushort.Parse(insideRaw);
			
			var fullNarrativeId = $"{networkId}:{deviceId}";

			WorldLocalNetworkData network = worldManager.World.LocalAreaNetworks.GetNarrativeObject(networkId);
			WorldComputerData device = worldManager.World.Computers.GetNarrativeObject(fullNarrativeId);
			
			WorldPortForwardingRule rule = worldManager.World.PortForwardingRules.FirstOrDefault(x => 
				x.LanId==network.InstanceId &&
				x.GlobalPort == outside);

			var ruleIsNew = false;
			if (rule.LanId != network.InstanceId)
			{
				ruleIsNew = true;
				rule.InstanceId = worldManager.GetNextObjectId();
			}

			rule.LanId = network.InstanceId;
			rule.GlobalPort = outside;
			rule.LocalPort = inside;
			rule.ComputerId = device.InstanceId;

			if (ruleIsNew)
				worldManager.World.PortForwardingRules.Add(rule);
			else
				worldManager.World.PortForwardingRules.Modify(rule);
		}
	}
}