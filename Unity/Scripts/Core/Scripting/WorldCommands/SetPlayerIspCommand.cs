using System;
using System.Linq;
using System.Threading.Tasks;
using Core.WorldData.Data;
using OS.Devices;

namespace Core.Scripting.WorldCommands
{
	public class SetPlayerIspCommand : WorldCommand
	{
		/// <inheritdoc />
		protected override async Task OnExecute(IWorldManager worldManager, IScriptExecutionContext context, ITextConsole console, string name, string[] args)
		{
			if (args.Length < 1)
				throw new InvalidOperationException("usage: setplayerisp <narrativeID>");

			string narrativeId = args[0];

			WorldInternetServiceProviderData isp = worldManager.World.InternetProviders.First(x => x.NarrativeId == narrativeId);
			IWorldDataObject<WorldPlayerData> playerData = worldManager.World.PlayerData;
			WorldPlayerData playerDataValue = playerData.Value;

			if (playerDataValue.PlayerInternetProvider != isp.InstanceId || playerDataValue.PublicNetworkAddress == 0)
			{
				playerDataValue.PlayerInternetProvider = default;
				playerDataValue.PublicNetworkAddress = 0;
				playerData.Value = playerDataValue;
			}

			playerDataValue.PlayerInternetProvider = isp.InstanceId;
			playerDataValue.PublicNetworkAddress = worldManager.GetNextPublicAddress(isp.InstanceId);
			playerData.Value = playerDataValue;
		}
	}
}