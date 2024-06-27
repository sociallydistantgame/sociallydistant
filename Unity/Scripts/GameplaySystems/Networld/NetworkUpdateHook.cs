#nullable enable
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Scripting;
using Core.WorldData.Data;
using GameplaySystems.Hacking.Assets;
using Modules;
using OS.Network;
using UnityEngine;
using UnityEngine.UI;

namespace GameplaySystems.Networld
{
	internal sealed class NetworkUpdateHook : IHookListener
	{
		private readonly IWorldManager worldHolder;

		public NetworkUpdateHook(IWorldManager worldHolder)
		{
			this.worldHolder = worldHolder;
		}
		
		/// <inheritdoc />
		public async Task ReceiveHookAsync(IGameContext game)
		{
			// assign the player's public IP
			IWorldDataObject<WorldPlayerData> playerData = worldHolder.World.PlayerData;
			ObjectId playerIsp = playerData.Value.PlayerInternetProvider;
			if (worldHolder.World.InternetProviders.Any(x => x.InstanceId == playerIsp))
			{
				if (playerData.Value.PublicNetworkAddress == 0)
				{
					WorldPlayerData playerValue = playerData.Value;
					playerValue.PublicNetworkAddress = worldHolder.GetNextPublicAddress(playerIsp);
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
			
			foreach (INetworkAsset asset in game.ContentManager.GetContentOfType<INetworkAsset>())
			{
				await asset.Build(worldHolder);
			}
		}
	}
}