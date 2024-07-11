using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.Core.ContentManagement;

public sealed class DefaultScriptImporter : ShellScriptImporter
{
    public override ShellScriptAsset? Import(IGameContext game, string[] shebangArgs, string scriptText)
    {
        throw new NotImplementedException();
    }
}