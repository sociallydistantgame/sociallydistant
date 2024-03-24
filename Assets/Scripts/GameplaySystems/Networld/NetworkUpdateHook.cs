#nullable enable
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Scripting;
using Core.WorldData.Data;
using GameplaySystems.Hacking.Assets;
using Modules;
using UnityEngine;
using UnityEngine.UI;

namespace GameplaySystems.Networld
{
	internal sealed class NetworkUpdateHook : IHookListener
	{
		private readonly WorldManagerHolder worldHolder;

		public NetworkUpdateHook(WorldManagerHolder worldHolder)
		{
			this.worldHolder = worldHolder;
		}
		
		/// <inheritdoc />
		public async Task ReceiveHookAsync(IGameContext game)
		{
			if (worldHolder.Value == null)
				return;
			
			// assign the player's public IP
			IWorldDataObject<WorldPlayerData> playerData = worldHolder.Value.World.PlayerData;
			ObjectId playerIsp = playerData.Value.PlayerInternetProvider;
			if (worldHolder.Value.World.InternetProviders.Any(x => x.InstanceId == playerIsp))
			{
				if (playerData.Value.PublicNetworkAddress == 0)
				{
					WorldPlayerData playerValue = playerData.Value;
					playerValue.PublicNetworkAddress = worldHolder.Value.GetNextPublicAddress(playerIsp);
					playerData.Value = playerValue;
				}
			}
			else
			{
				if (playerData.Value.PublicNetworkAddress != 0)
				{
					WorldPlayerData playerValue = playerData.Value;
					playerValue.PublicNetworkAddress = 0;
					playerData.Value = playerValue;
				}
			}
			
			NetworkAsset[]? assets = Resources.LoadAll<NetworkAsset>("Networks");

			foreach (NetworkAsset asset in assets)
			{
				await Task.Yield();

				asset.Build(worldHolder.Value);
			}
		}
	}
}