using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.FloatingTools.FileManager;

public class FileManagerProgram : IProgram
{
    public string Name { get; } = "browse";
    public string WindowTitle { get; } = "File Manager";
    public CompositeIcon Icon { get; } = MaterialIcons.FolderOpen;

    public void InstantiateIntoWindow(
        ISystemProcess process,
        IContentPanel window,
        ITextConsole console,
        string[] args
    )
    {
        ProgramController.Create<FileManagerController>(process, window, console, args);
    }
}