using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.Core.UI.Shell;

public abstract class TrayAction : IGameContent
{
    private readonly IGameContext context;

    // TODO: Allow the UI to detect icon changes.
    public CompositeIcon? Icon { get; protected set; }

    protected IGameContext Context => context;
    
    public TrayAction(IGameContext context)
    {
        this.context = context;
    }

    public virtual bool CanUseInGameMode(GameMode gameMode)
    {
        return true;
    }
    
    public abstract void Invoke();
}