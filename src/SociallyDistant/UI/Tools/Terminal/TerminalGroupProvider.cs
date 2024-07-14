using SociallyDistant.Architecture;
using SociallyDistant.Core.Shell;

namespace SociallyDistant.UI.Tools.Terminal;

public class TerminalGroupProvider : IToolProvider
{
    public ITabbedToolDefinition CreateToolDefinition()
    {
        return new TerminalTool();
    }
}