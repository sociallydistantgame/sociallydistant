using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;
using SociallyDistant.GameplaySystems.Hacking.Assets;

namespace SociallyDistant.ScriptImpoers;

[Shebang("network")]
public sealed class NetworkImporter : ShellScriptImporter
{
    public override ShellScriptAsset? Import(IGameContext game, string[] shebangArgs, string scriptText)
    {
        string narrativeId = string.Join(" ", shebangArgs).Trim();
        var asset = new NetworkAsset(narrativeId, scriptText);
        return asset;
    }
}