using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;
using SociallyDistant.GameplaySystems.Social;

namespace SociallyDistant.ScriptImpoers;

[Shebang("npc")]
public sealed class NpcGeneratorImporter : ShellScriptImporter
{
    public override ShellScriptAsset? Import(IGameContext game, string[] shebangArgs, string scriptText)
    {
        var narrativeId = string.Join(" ", shebangArgs).Trim();
        var asset = new NpcGeneratorScript(narrativeId, scriptText);
        return asset;
    }
}