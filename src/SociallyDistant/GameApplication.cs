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

    protected override void Run()
    {
        game.Run();
    }

    public void Dispose()
    {
        game.Dispose();
    }
}