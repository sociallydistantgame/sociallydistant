using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Tools.Terminal;

internal sealed class TerminalProgram : IProgram
{
    public string Name => "terminal";
    public string WindowTitle => "Terminal";
    public CompositeIcon Icon { get; } = MaterialIcons.Terminal;

    public void InstantiateIntoWindow(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args)
    {
        ProgramController.Create<TerminalProgramController>(process, window, console, args);
    }
}