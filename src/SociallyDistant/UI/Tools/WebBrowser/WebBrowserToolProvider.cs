using SociallyDistant.Core.Shell;

namespace SociallyDistant.UI.Tools.WebBrowser;

public sealed class WebBrowserToolProvider : IToolProvider
{
    public ITabbedToolDefinition CreateToolDefinition()
    {
        return new WebBrowserTool();
    }
}