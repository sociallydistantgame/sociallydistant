using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;
using SociallyDistant.GameplaySystems.Chat;
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

[Shebang("chat")]
public class ChatScriptImporter : ShellScriptImporter
{
    public override ShellScriptAsset? Import(IGameContext game, string[] shebangArgs, string scriptText)
    {
        if (shebangArgs.Length == 0)
            throw new InvalidOperationException("Conversation ID expected in the shebang, i.e '#!chat <conversation_id>'");
        
        var asset = new ChatConversationAsset(shebangArgs[0], scriptText);
        return asset;
    }
}