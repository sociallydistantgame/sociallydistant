using SociallyDistant.Core.Shell;

namespace SociallyDistant.UI.Tools.Email;

public sealed class EmailGroupProvider : IToolProvider
{
    public ITabbedToolDefinition CreateToolDefinition()
    {
        return new EmailTool();
    }
}