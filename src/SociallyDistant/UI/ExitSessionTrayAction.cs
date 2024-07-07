using SociallyDistant.Core.Modules;
using SociallyDistant.Core.UI.Shell;

namespace SociallyDistant.UI;

internal sealed class ExitSessionTrayAction : TrayAction
{
    public ExitSessionTrayAction(IGameContext context) : base(context)
    {
    }

    public override void Invoke()
    {
    }
}