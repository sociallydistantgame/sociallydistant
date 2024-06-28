#nullable enable

using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.OS.Network;
using SociallyDistant.Core.Scripting;
using SociallyDistant.Core.Scripting.Instructions;
using SociallyDistant.Core.Scripting.Parsing;
using SociallyDistant.GamePlatform;

namespace SociallyDistant.GameplaySystems.Hacking.Assets
{
	public class NetworkAsset : INetworkAsset
	{
		
		private string scriptText = string.Empty;

		
		private string narrativeId = string.Empty;

		
		private string networkName = string.Empty;

		
		private string ispId = string.Empty;

		
		private List<string> domains = new List<string>();

		
		private Subnet subnet;

		
		private List<DeviceName> devices = new List<DeviceName>();

		
		private List<ListenerData> listeners = new List<ListenerData>();

		
		private List<PortForwardingData> forwardingRules = new List<PortForwardingData>();

		
		private string? webProxyServer;

		
		private List<ProxyForwardData> proxyForwardData = new List<ProxyForwardData>();

		
		private List<UserData> users = new List<UserData>();

		private ShellInstruction? scriptTree;

		public string NarrativeId
		{
			get => narrativeId;
#if UNITY_EDITOR
			set => this.narrativeId = value;
#endif
		}

		public string NetworkName => networkName;

		#if UNITY_EDITOR
		public void SetScriptText(string text)
		{
			this.scriptText = text;

			Task.Run(async () =>
			{
				await RebuildScriptTree();
			}).Wait();
		}
		#endif

		public async Task RebuildScriptTree()
		{
			var context = new UserScriptExecutionContext();
			var console = new UnityTextConsole();

			var runner = new InteractiveShell(context);
			runner.Setup(console);

			ShellInstruction script = await runner.ParseScript(this.scriptText);

			var call = new SingleInstruction(new CommandData(new TextArgumentEvaluator("build"), Array.Empty<IArgumentEvaluator>(), FileRedirectionType.None, null));

			var finalScript = new SequentialInstruction(new[] { script, call });

			this.scriptTree = finalScript;
		}

		public async Task Build(IWorldManager worldManager)
		{
			if (this.scriptTree == null)
				await this.RebuildScriptTree();

			var context = new UserScriptExecutionContext();

			context.ModuleManager.RegisterModule(new NetworkScriptFunctions(worldManager, narrativeId));

			var console = new UnityTextConsole();

			var runner = new InteractiveShell(context);
			runner.Setup(console);

			await runner.RunParsedScript(this.scriptTree!);
		}
		

		public async Task BuildOld(IWorldManager worldManager)
		{
			await Task.Yield();
			IWorld world = worldManager.World;

			WorldInternetServiceProviderData isp = world.InternetProviders.FirstOrDefault(x => x.NarrativeId == ispId);
			if (isp.NarrativeId != ispId)
			{
				isp.InstanceId = worldManager.GetNextObjectId();
				isp.NarrativeId = ispId;
				isp.Name = $"Unknown ISP {ispId}";
				isp.CidrNetwork = worldManager.GetNextIspRange();
				
				world.InternetProviders.Add(isp);
			}
			
			WorldLocalNetworkData localNetwork = world.LocalAreaNetworks.FirstOrDefault(x => x.NarrativeId == narrativeId);
			
			var networkIsNew = false;
			if (localNetwork.NarrativeId != narrativeId)
			{
				networkIsNew = true;
				localNetwork.InstanceId = worldManager.GetNextObjectId();
				localNetwork.NarrativeId = narrativeId;
			}

			if (localNetwork.ServiceProviderId != isp.InstanceId)
			{
				localNetwork.PublicNetworkAddress = 0;
			}
			
			if (localNetwork.PublicNetworkAddress == 0)
			{
				localNetwork.PublicNetworkAddress = worldManager.GetNextPublicAddress(isp.InstanceId);
			}
			
			localNetwork.Name = this.networkName;
			localNetwork.ServiceProviderId = isp.InstanceId;
			
			if (networkIsNew)
				world.LocalAreaNetworks.Add(localNetwork);
			else 
				world.LocalAreaNetworks.Modify(localNetwork);

			foreach (DeviceName deviceName in devices)
			{
				var deviceId = $"{localNetwork.NarrativeId}:{deviceName.narrativeId}";

				WorldComputerData deviceData = world.Computers.FirstOrDefault(x => x.NarrativeId == deviceId);

				var deviceIsNew = false;
				if (deviceData.NarrativeId != deviceId)
				{
					deviceIsNew = true;
					deviceData.InstanceId = worldManager.GetNextObjectId();
					deviceData.NarrativeId = deviceId;
					deviceData.MacAddress = NetUtility.GetRandomMacAddress();
				}
				
				deviceData.HostName = deviceName.hostName;
				
				if (deviceIsNew)
					world.Computers.Add(deviceData);
				else
					world.Computers.Modify(deviceData);

				WorldNetworkConnection deviceConnection = world.NetworkConnections.FirstOrDefault(x => x.ComputerId == deviceData.InstanceId);

				bool connectionIsNew = deviceConnection.ComputerId != deviceData.InstanceId;

				if (connectionIsNew)
				{
					deviceConnection.InstanceId = worldManager.GetNextObjectId();
				}

				deviceConnection.ComputerId = deviceData.InstanceId;
				deviceConnection.LanId = localNetwork.InstanceId;
				
				if (connectionIsNew)
					world.NetworkConnections.Add(deviceConnection);
				else
					world.NetworkConnections.Modify(deviceConnection);

				var keptHackables = new List<ObjectId>();
				foreach (ListenerData listener in listeners.Where(x => x.deviceId == deviceName.narrativeId))
				{
					WorldHackableData hackable = world.Hackables.FirstOrDefault(x => x.ComputerId==deviceData.InstanceId && x.Port==listener.port);

					var hackableIsNew = false;
					if (hackable.ComputerId != deviceData.InstanceId)
					{
						hackableIsNew = true;
						hackable.InstanceId = worldManager.GetNextObjectId();
						hackable.ComputerId = deviceData.InstanceId;
					}
					
					keptHackables.Add(hackable.InstanceId);

					hackable.Port = listener.port;
					hackable.ServerType = listener.serverTYpe;
					
					if (hackableIsNew)
						world.Hackables.Add(hackable);
					else
						world.Hackables.Modify(hackable);
				}

				WorldHackableData[] hackablesToRemove = world.Hackables.Where(x => x.ComputerId == deviceData.InstanceId && !keptHackables.Contains(x.InstanceId)).ToArray();
				
				foreach (WorldHackableData hackableToRemove in hackablesToRemove)
					world.Hackables.Remove(hackableToRemove);

				var rulesToKeep = new List<ObjectId>();

				foreach (PortForwardingData ruleData in forwardingRules.Where(x => x.deviceId == deviceName.narrativeId))
				{
					WorldHackableData hackable = world.Hackables.FirstOrDefault(x => x.ComputerId == deviceData.InstanceId && x.Port == ruleData.insidePort);
					if (hackable.ComputerId != deviceData.InstanceId)
						continue;

					WorldPortForwardingRule rule = world.PortForwardingRules.FirstOrDefault(x => 
						x.LanId==localNetwork.InstanceId &&
						x.GlobalPort == ruleData.outsidePort);

					var ruleIsNew = false;
					if (rule.LanId != localNetwork.InstanceId)
					{
						ruleIsNew = true;
						rule.InstanceId = worldManager.GetNextObjectId();
					}
					
					rulesToKeep.Add(rule.InstanceId);

					rule.LanId = localNetwork.InstanceId;
					rule.GlobalPort = ruleData.outsidePort;
					rule.LocalPort = ruleData.insidePort;
					rule.ComputerId = deviceData.InstanceId;

					if (ruleIsNew)
						world.PortForwardingRules.Add(rule);
					else
						world.PortForwardingRules.Modify(rule);
				}

				WorldPortForwardingRule[] rulesToDelete = world.PortForwardingRules.Where(x => x.LanId == localNetwork.InstanceId && x.ComputerId == deviceData.InstanceId && !rulesToKeep.Contains(x.InstanceId)).ToArray();

				foreach (WorldPortForwardingRule rule in rulesToDelete)
					world.PortForwardingRules.Remove(rule);
			}

			if (NetUtility.TryParseCidrNotation(isp.CidrNetwork, out Subnet ispSubnet))
			{
				uint networkAddress = (ispSubnet.networkAddress & ispSubnet.mask) | (localNetwork.PublicNetworkAddress & ~ispSubnet.mask);
				
				foreach (string domain in domains)
				{
					WorldDomainNameData domainData = world.Domains.FirstOrDefault(x => x.RecordName == domain);

					var domainIsNew = false;
					if (domainData.RecordName != domain)
					{
						domainIsNew = true;
						domainData.InstanceId = worldManager.GetNextObjectId();
						domainData.RecordName = domain;
					}
					
					domainData.Address = networkAddress;

					if (domainIsNew)
						world.Domains.Add(domainData);
					else
						world.Domains.Modify(domainData);
				}
			}
		}
		
		#if UNITY_EDITOR

		public void SetNarrativeId(string narrativeId)
		{
			this.narrativeId = narrativeId;
		}

		public void SetFriendlyName(string friendlyName)
		{
			this.networkName = friendlyName;
		}

		public void SetIsp(string ispId)
		{
			this.ispId = ispId;
		}
		
		public void AddDomainName(string domainName)
		{
			if (this.domains.Contains(domainName))
				return;

			this.domains.Add(domainName);
		}

		public void SetSubnet(Subnet subnet)
		{
			this.subnet = subnet;
		}

		public void AddDevice(string narrativeId, string hostName)
		{
			if (devices.Any(x => x.narrativeId == narrativeId))
				throw new InvalidOperationException($"Device with narrativeId {narrativeId} already exists.");

			devices.Add(new DeviceName
			{
				narrativeId = narrativeId,
				hostName = hostName
			});
		}

		public void AddListener(string deviceId, ushort port, ServerType serverType)
		{
			if (devices.All(x => x.narrativeId != deviceId))
				throw new InvalidOperationException($"Device with narrativeId {deviceId} not created yet.");

			if (listeners.Any(x => x.deviceId == deviceId && x.port == port))
				throw new InvalidOperationException($"A listener with the same deviceId {deviceId} and port {port} was already created in this network!");

			if (serverType == ServerType.Unknown)
				throw new InvalidOperationException("Unknown server type.");
			
			this.listeners.Add(new ListenerData
			{
				deviceId = deviceId,
				port = port,
				serverTYpe = serverType
			});
		}

		public void PortForward(ushort outsidePort, string deviceId, ushort insidePort)
		{
			if (!listeners.Any(x => x.deviceId == deviceId && x.port==insidePort))
				throw new InvalidOperationException($"No listener was created for deviceId {deviceId} on inside port {insidePort}!");

			if (forwardingRules.Any(x => x.outsidePort == outsidePort))
				throw new InvalidOperationException($"Outside port {outsidePort} is already forwarded!");
			
			this.forwardingRules.Add(new PortForwardingData
			{
				outsidePort = outsidePort,
				insidePort = insidePort,
				deviceId = deviceId
			});
		}

		public void SetProxyDevice(string proxyDevice)
		{
			if (devices.All(x => x.narrativeId != proxyDevice))
				throw new InvalidOperationException($"Cannot set {proxyDevice} as the network's reverse proxy server because the target device hasn't been created yet.");
            
			this.webProxyServer = proxyDevice;
		}

		public void ProxyForward(string domainName, string targetServer)
		{
			if (proxyForwardData.Any(x => x.outsideDomain == domainName))
				throw new InvalidOperationException($"The domain name {domainName} is already being forwarded by the reverse proxy.");

			if (devices.All(x => x.narrativeId != targetServer))
				throw new InvalidOperationException($"Cannot proxy {domainName} to the target server {targetServer} because the device hasn't been created yet.");

			this.proxyForwardData.Add(new ProxyForwardData()
			{
				outsideDomain = domainName,
				destinationDevice = targetServer
			});
		}

		public void AddUser(string deviceId, string username, UserType userType)
		{
			if (!devices.Any(x => x.narrativeId == deviceId))
				throw new InvalidOperationException($"Cannot create user for deviceId {deviceId} because the device hasn't yet been created.");

			if (users.Any(x => x.deviceId == deviceId && x.userName == username))
				throw new InvalidOperationException($"User with username {username} on device {deviceId} already exists.");
			
			this.users.Add(new UserData
			{
				deviceId = deviceId,
				userName = username,
				userType = userType
			});
		}
		
		#endif

		[Serializable]
		private struct DeviceName
		{
			public string narrativeId;
			public string hostName;
		}

		[Serializable]
		private struct ListenerData
		{
			public string deviceId;
			public ushort port;
			public ServerType serverTYpe;
		}

		[Serializable]
		private struct PortForwardingData
		{
			public string deviceId;
			public ushort outsidePort;
			public ushort insidePort;
		}

		[Serializable]
		private struct ProxyForwardData
		{
			public string outsideDomain;
			public string destinationDevice;
		}

		[Serializable]
		private struct UserData
		{
			public string deviceId;
			public string userName;
			public UserType userType;
		}
		
		public enum UserType
		{
			Admin,
			Limited
		}
	}
}