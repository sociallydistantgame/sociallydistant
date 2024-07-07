using SociallyDistant.Core.Modules;
using SociallyDistant.Core.UI.Shell;

namespace SociallyDistant.UI;

internal sealed class ExitGameTrayAction : TrayAction
{
    public ExitGameTrayAction(IGameContext context) : base(context)
    {
    }

    public override void Invoke()
    {
    }
}