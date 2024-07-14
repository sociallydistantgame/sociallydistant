using SociallyDistant.Core.Shell;

namespace SociallyDistant.UI.Tools.Chat;

public class ChatGroupProvider : IToolProvider
{
    public ITabbedToolDefinition CreateToolDefinition()
    {
        return new ChatToolDefinition();
    }
}