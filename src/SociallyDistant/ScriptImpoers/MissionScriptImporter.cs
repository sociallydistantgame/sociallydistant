using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;
using SociallyDistant.GameplaySystems.Missions;

namespace SociallyDistant.ScriptImpoers;

[Shebang("mission")]
public sealed class MissionScriptImporter : ShellScriptImporter
{
    public override ShellScriptAsset? Import(IGameContext game, string[] shebangArgs, string scriptText)
    {
        return new MissionScriptAsset(string.Join(" ", shebangArgs), scriptText);
    }
}