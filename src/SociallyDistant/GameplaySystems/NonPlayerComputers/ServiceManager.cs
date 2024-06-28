#nullable enable
using System.Diagnostics;
using Serilog;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.GameplaySystems.NonPlayerComputers
{
	public sealed class ServiceManager
	{
		private readonly ISystemProcess initProcess;
		private readonly SociallyDistantGame gameManager;
		private readonly Dictionary<string, INetworkService> runningServices = new();

		public ServiceManager(ISystemProcess initProcess)
		{
			this.gameManager = SociallyDistantGame.Instance;
			this.initProcess = initProcess;
		}
        
		public void Update()
		{
			foreach (string key in runningServices.Keys)
			{
				runningServices[key].Update();
			}
		}

		public void UpdateServices(IEnumerable<NetworkServiceData> servicesToRun)
		{
			Dictionary<string, NetworkServiceData> serviceDictionary = servicesToRun.ToDictionary(x => x.Id, x => x);

			string[] hitList = runningServices.Keys.Where(x => !serviceDictionary.ContainsKey(x) || !serviceDictionary[x].Enabled).ToArray();

			Dictionary<string, INetworkServiceProvider> serviceProviders = gameManager.ContentManager.GetContentOfType<INetworkServiceProvider>()
				.Where(x => !hitList.Contains(x.Id))
				.ToDictionary(x => x.Id, x => x);
			
			foreach (string murderCandidate in hitList)
			{
				if (!runningServices.ContainsKey(murderCandidate))
					continue;
				
				INetworkService service = runningServices[murderCandidate];

				service.Stop();

				runningServices.Remove(murderCandidate);
			}

			foreach (string key in serviceDictionary.Keys)
			{
				if (runningServices.ContainsKey(key))
					continue;
				
				if (!serviceProviders.TryGetValue(key, out INetworkServiceProvider provider))
				{
					Log.Error($"Attempt to start a service on an NPC computer with {key}, but no provider was found in ContentManager. Missing script mod?");
					continue;
				}

				INetworkService service = provider.CreateService(this.initProcess);

				if (service is IUserSpecifiedPort port)
					port.Port = serviceDictionary[key].Port;
				
				service.Start();
				
				this.runningServices.Add(key, service);
			}
		}
	}
}