using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Tools.WebBrowser;

public sealed class WebBrowserProgram : IProgram
{
    public string Name { get; } = "airhawk";
    public string WindowTitle { get; } = "Airhawk Web Browser";
    public CompositeIcon Icon { get; } = MaterialIcons.Web;

    public void InstantiateIntoWindow(
        ISystemProcess process,
        IContentPanel window,
        ITextConsole console,
        string[] args
    )
    {
        ProgramController.Create<WebBrowserController>(process, window, console, args);
    }
}