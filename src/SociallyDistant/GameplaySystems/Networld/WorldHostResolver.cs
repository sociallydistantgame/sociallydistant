#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.OS.Network;

namespace SociallyDistant.GameplaySystems.Networld
{
	public sealed class WorldHostResolver : IHostNameResolver
	{
		private readonly IWorldManager worldHolder;

		public WorldHostResolver(IWorldManager holder)
		{
			this.worldHolder = holder;
		}

		/// <inheritdoc />
		public bool IsValidSubnet(uint address)
		{
			foreach (WorldInternetServiceProviderData isp in worldHolder.World.InternetProviders)
			{
				if (!NetUtility.TryParseCidrNotation(isp.CidrNetwork, out Subnet ispSubnet))
					continue;

				// Address isn't in the ISP.
				if ((address & ispSubnet.mask) != ispSubnet.networkAddress)
					continue;

				if (worldHolder.World.PlayerData.Value.PlayerInternetProvider == isp.InstanceId)
				{
					// check the player's LAN to see if it matches
					uint lanAddress = (ispSubnet.networkAddress & ispSubnet.mask) | (worldHolder.World.PlayerData.Value.PublicNetworkAddress & ~ispSubnet.mask);
					if (lanAddress == address)
						return true;
				}
				
				foreach (WorldLocalNetworkData lan in worldHolder.World.LocalAreaNetworks)
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
			WorldDomainNameData? firstRecord = worldHolder?
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

			WorldDomainNameData? firstRecord = worldHolder?
				.World
				.Domains
				.FirstOrDefault(x => x.RecordName == hostname);

			if (string.IsNullOrWhiteSpace(firstRecord?.RecordName))
				return null;
			
			return firstRecord?.Address;
		}
	}
}