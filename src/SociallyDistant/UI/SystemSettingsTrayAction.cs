using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Shell;

namespace SociallyDistant.UI;

internal sealed class SystemSettingsTrayAction : TrayAction
{
    public SystemSettingsTrayAction(IGameContext context) : base(context)
    {
        Icon = MaterialIcons.Settings;
    }

    public override void Invoke()
    {
        Context.Shell.OpenSettings();
    }
}