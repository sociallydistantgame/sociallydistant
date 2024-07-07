using SociallyDistant.Architecture;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Tools.Terminal;

public class TerminalGroupProvider : IToolProvider
{
    public ITabbedToolDefinition CreateToolDefinition()
    {
        return new TerminalTool();
    }
}

internal class TerminalTool : ITabbedToolDefinition
{
    public IProgram Program => new TerminalProgram();
    public bool AllowUserTabs => true;
    public INotificationGroup? NotificationGroup => null;
}

internal sealed class TerminalProgram : IProgram
{
    public string BinaryName => "terminal";
    public string WindowTitle => "Terminal";
    public CompositeIcon Icon { get; } = MaterialIcons.Terminal;

    public void InstantiateIntoWindow(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args)
    {
        ProgramController.Create<TerminalProgramController>(process, window, console, args);
    }
}