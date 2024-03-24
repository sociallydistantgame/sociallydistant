#nullable enable
using System.Linq;
using Core;
using Core.WorldData.Data;
using OS.Network;

namespace GameplaySystems.Networld
{
	public sealed class WorldHostResolver : IHostNameResolver
	{
		private readonly WorldManagerHolder worldHolder;

		public WorldHostResolver(WorldManagerHolder holder)
		{
			this.worldHolder = holder;
		}

		/// <inheritdoc />
		public bool IsValidSubnet(uint address)
		{
			// No Internet!
			if (worldHolder.Value == null)
				return false;


			foreach (WorldInternetServiceProviderData isp in worldHolder.Value.World.InternetProviders)
			{
				if (!NetUtility.TryParseCidrNotation(isp.CidrNetwork, out Subnet ispSubnet))
					continue;

				// Address isn't in the ISP.
				if ((address & ispSubnet.mask) != ispSubnet.networkAddress)
					continue;

				if (worldHolder.Value.World.PlayerData.Value.PlayerInternetProvider == isp.InstanceId)
				{
					// check the player's LAN to see if it matches
					uint lanAddress = (ispSubnet.networkAddress & ispSubnet.mask) | (worldHolder.Value.World.PlayerData.Value.PublicNetworkAddress & ~ispSubnet.mask);
					if (lanAddress == address)
						return true;
				}
				
				foreach (WorldLocalNetworkData lan in worldHolder.Value.World.LocalAreaNetworks)
				{
					// Address of a LAN!
					uint lanAddress = (ispSubnet.networkAddress & ispSubnet.mask) | (lan.PublicNetworkAddress & ~ispSubnet.mask);
					if (lanAddress == address)
						return true;
				}
			}

			return false;
		}

		/// <inheritdoc />
		public string? ReverseHostLookup(uint networkAddress)
		{
			WorldDomainNameData? firstRecord = worldHolder.Value?
				.World
				.Domains
				.FirstOrDefault(x => x.Address == networkAddress);
		
			if (string.IsNullOrWhiteSpace(firstRecord?.RecordName))
				return null;
			
			return firstRecord?.RecordName;
		}

		/// <inheritdoc />
		public uint? HostLookup(string hostname)
		{
			// IP addresses always take precedence over hostnames!
			if (NetUtility.TryParseNetworkAddress(hostname, out uint parsedAddress))
				return parsedAddress;

			WorldDomainNameData? firstRecord = worldHolder.Value?
				.World
				.Domains
				.FirstOrDefault(x => x.RecordName == hostname);

			if (string.IsNullOrWhiteSpace(firstRecord?.RecordName))
				return null;
			
			return firstRecord?.Address;
		}
	}
}