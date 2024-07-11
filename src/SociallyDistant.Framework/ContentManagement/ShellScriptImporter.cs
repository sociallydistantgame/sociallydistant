using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.Core.ContentManagement;

public abstract class ShellScriptImporter
{
    public abstract ShellScriptAsset? Import(IGameContext game, string[] shebangArgs, string scriptText);
}