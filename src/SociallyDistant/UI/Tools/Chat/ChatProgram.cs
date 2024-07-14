using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ChatProgram : IProgram
{
    public string Name => "chat";
    public string WindowTitle => "Chat";
    public CompositeIcon Icon { get; } = MaterialIcons.Chat;

    public void InstantiateIntoWindow(
        ISystemProcess process,
        IContentPanel window,
        ITextConsole console,
        string[] args
    )
    {
        ProgramController.Create<ChatProgramController>(process, window, console, args);
    }
}