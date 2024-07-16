using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.ScriptImpoers;

[Shebang("hook")]
public sealed class HookScriptImporter : ShellScriptImporter
{
    public override ShellScriptAsset? Import(IGameContext game, string[] shebangArgs, string scriptText)
    {
        string hookId = shebangArgs[0];

        var hook = new HookScript(hookId, scriptText);
        return hook;
    }
}