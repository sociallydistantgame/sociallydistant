using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.Core.Core.Scripting;

public sealed class WorldScriptCommands : ScriptModule
{
    private readonly IGameContext game;

    public WorldScriptCommands(IGameContext game)
    {
        this.game = game;
    }

    [Function("hookexec")]
    public async Task ExecuteHook(string hookName)
    {
        await game.ScriptSystem.RunHookAsync(hookName);
    }

    [Function("worldflag")]
    public int GetWorldFlagState(string flag)
    {
        if (flag.StartsWith("!"))
        {
            return !game.WorldManager.World.WorldFlags.Contains(flag.Substring(1)) ? 0 : 1;
        }

        return game.WorldManager.World.WorldFlags.Contains(flag) ? 0 : 1;
    }
    
    [Function("setplayerisp")]
    public void SetPlayerIsp(string narrativeId)
    {
        WorldInternetServiceProviderData isp = game.WorldManager.World.InternetProviders.First(x => x.NarrativeId == narrativeId);
        IWorldDataObject<WorldPlayerData> playerData = game.WorldManager.World.PlayerData;
        WorldPlayerData playerDataValue = playerData.Value;

        if (playerDataValue.PlayerInternetProvider != isp.InstanceId || playerDataValue.PublicNetworkAddress == 0)
        {
            playerDataValue.PlayerInternetProvider = default;
            playerDataValue.PublicNetworkAddress = 0;
            playerData.Value = playerDataValue;
        }

        playerDataValue.PlayerInternetProvider = isp.InstanceId;
        playerDataValue.PublicNetworkAddress = game.WorldManager.GetNextPublicAddress(isp.InstanceId);
        playerData.Value = playerDataValue;
    }

    [Function("spawnisp")]
    public void SpawnISP(string narrativeId, string ispName)
    {
        var isp = game.WorldManager.World.InternetProviders.FirstOrDefault(x => x.NarrativeId == narrativeId);

        var ispIsNew = false;
        if (isp.NarrativeId != narrativeId)
        {
            ispIsNew = true;
            isp.InstanceId = game.WorldManager.GetNextObjectId();
            isp.CidrNetwork = game.WorldManager.GetNextIspRange();
            isp.NarrativeId = narrativeId;
        }

        isp.Name = ispName;

        if (ispIsNew)
        {
            game.WorldManager.World.InternetProviders.Add(isp);
        }
        else
        {
            game.WorldManager.World.InternetProviders.Modify(isp);
        }
    }
}