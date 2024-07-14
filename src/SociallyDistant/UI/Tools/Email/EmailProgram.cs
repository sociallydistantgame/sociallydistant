using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Tools.Email;

public sealed class EmailProgram : IProgram
{
    public string Name { get; } = "email";
    public string WindowTitle { get; } = "Email";
    public CompositeIcon Icon { get; } = MaterialIcons.Email;

    public void InstantiateIntoWindow(
        ISystemProcess process,
        IContentPanel window,
        ITextConsole console,
        string[] args
    )
    {
        ProgramController.Create<EmailProgramController>(process, window, console, args);
    }
}