using SociallyDistant;
using SociallyDistant.Core.OS.FileSystems.Host;
using SociallyDistant.OS.Devices;
using SociallyDistant.Player;
using SociallyDistant.VfsMapping;

public sealed class PlayerInitialization
{
    private readonly PlayerManager       player;
    private readonly SociallyDistantGame game;
	
    internal PlayerInitialization(PlayerManager player, SociallyDistantGame game)
    {
        this.player = player;
        this.game = game;
    }

    public async Task InitializeSystem()
    {
        var filesystem = player.Computer.GetFileSystem(player.InitProcess.User);

        var playerDataPath = Path.Combine(SociallyDistantGame.GameDataPath, "test_filesystem");
        if (!string.IsNullOrWhiteSpace(game.CurrentSaveDataDirectory))
            playerDataPath = Path.Combine(game.CurrentSaveDataDirectory, "devices", "player");
        
        // Mount filesystems
        filesystem.Mount("/bin",     new ScriptableCommandVfsMap().GetFileSystem());
        filesystem.Mount("/usr/bin", new ProgramsListFileSystem().GetFileSystem());
        //filesystem.Mount("/etc",     new HostJail(Path.Combine(playerDataPath,                        "etc")));
        filesystem.Mount("/root",    new HostJail(Path.Combine(playerDataPath,                        "root")));
        filesystem.Mount("/home",    new HomeFileSystem(player.Computer, Path.Combine(playerDataPath, "home")));
		
        // bare minimum environment vars needed for the game to work
        // Without this, shells can't find programs/commands.
        player.InitProcess.Environment["PATH"] = "/bin:/usr/bin:/sbin:/usr/sbin";
		
        // Fallback PS1 in case none of the rest of this code does anything
        player.InitProcess.Environment["PS1"] = "%u@%h:%w%$ ";
        
        
    }
}