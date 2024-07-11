using System.Reactive.Linq;
using System.Reactive.Subjects;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.UI.Shell;

namespace SociallyDistant.UI;

internal sealed class TrayModel : IHookListener
{
    private readonly SociallyDistantGame                  game;
    private readonly List<TrayAction>                     actions        = new();
    private readonly Subject<IEnumerable<TrayAction>>     actionsSubject = new();
    private readonly IObservable<IEnumerable<TrayAction>> trayObservable;
    
    private GameMode currentGameMode;

    public IObservable<IEnumerable<TrayAction>> TrayActions => trayObservable;
    
    internal TrayModel()
    {
        this.game = SociallyDistantGame.Instance;
        this.game.ScriptSystem.RegisterHookListener(CommonScriptHooks.AfterContentReload, this);
        trayObservable = Observable.Create<IEnumerable<TrayAction>>((observer) =>
        {
            observer.OnNext(actions);
            return actionsSubject.Subscribe(observer);
        });
    }

    
    
    internal void UpdateGameMode(GameMode gameMode)
    {
        currentGameMode = gameMode;
        RefreshTrayActions();
    }

    public Task ReceiveHookAsync(IGameContext context)
    {
        RefreshTrayActions();
        return Task.CompletedTask;
    }

    private void RefreshTrayActions()
    {
        actions.Clear();
        
        foreach (TrayAction action in game.ContentManager.GetContentOfType<TrayAction>().Where(x=>x.CanUseInGameMode(currentGameMode)))
        {
            actions.Add(action);
        }

        actionsSubject.OnNext(actions);
    }
}