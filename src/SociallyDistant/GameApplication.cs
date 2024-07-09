using SociallyDistant.Core.Modules;

namespace SociallyDistant;

internal sealed class GameApplication : Application, 
    IDisposable
{
    private readonly SociallyDistantGame game;
	
    public override IGameContext Context => game;

    public GameApplication()
    {
        game = new SociallyDistantGame();
    }

    public void Run()
    {
        game.Run();
    }

    public void Dispose()
    {
        game.Dispose();
    }
}