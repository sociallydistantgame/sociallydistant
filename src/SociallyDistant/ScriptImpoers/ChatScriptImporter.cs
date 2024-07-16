using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.Scripting.Instructions;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.GameplaySystems.Chat;

namespace SociallyDistant.ScriptImpoers;

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