using Microsoft.Xna.Framework;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.GameplaySystems.Networld;

internal sealed class NetworkController : GameComponent
{
    private readonly WorldHostResolver hostResolver;
    private readonly CoreRouter coreRouter;
    private readonly NetworkSimulationController simulationController;
    private readonly IGameContext context;

    internal NetworkSimulationController Simulation => simulationController;
    
    internal NetworkController(SociallyDistantGame game) : base(game)
    {
        context = game;
        hostResolver = new WorldHostResolver(game.WorldManager);
        coreRouter = new CoreRouter(this.hostResolver);
        simulationController = new NetworkSimulationController(coreRouter);
    }

    public override void Initialize()
    {
        base.Initialize();

        context.GameModeObservable.Subscribe(OnGameModeChanged);
    }

    private void OnGameModeChanged(GameMode newGameMode)
    {
        if (newGameMode == GameMode.OnDesktop)
            simulationController.EnableSimulation();
        else
            simulationController.DisableSimulation();
    }
}