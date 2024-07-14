using System.Reflection;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.Core.Programs;

public abstract class ProgramController
{
    private readonly ProgramContext context;

    public string CurrentDirectory
    {
        get => context.Process.WorkingDirectory;
        set => context.Process.WorkingDirectory = value;
    }

    protected IVirtualFileSystem FileSystem => context.Process.User.Computer.GetFileSystem(context.Process.User);
    
    public string WindowTitle
    {
        get => context.Window.Title;
        set => context.Window.Title = value;
    }

    protected IUser User => context.Process.User;
    protected ISystemProcess Process => context.Process;
    
    protected ProgramController(ProgramContext context)
    {
        this.context = context;
        WindowTitle = GetType().Name;
    }

    protected abstract void Main();

    public static T Create<T>(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args) where T : ProgramController
    {
        var type = typeof(T);
        if (type.IsAbstract)
            throw new InvalidOperationException(
                $"Creating the program controller of type {type.FullName} isn't possible, because the class is abstract.");

        var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, new[] { typeof(ProgramContext) });
        if (constructor == null)
            throw new InvalidOperationException(
                $"Cannot find a valid constructor for the program controller {type.FullName}. Make sure there is one constructor taking a single ProgramContext parameter and that it is private.");
        
        var context = new ProgramContext(process, window, console, args);

        var instance = constructor.Invoke([context]) as T;
        if (instance == null)
            throw new InvalidOperationException($"Failed to instantiate {type.FullName}.");

        instance.Main();

        return instance;
    }

    protected void CloseWindow(int exitCode = 0)
    {
        Process.Kill(exitCode);
        this.context.Window.ForceClose();
    }
    
    protected sealed class ProgramContext
    {
        private readonly ISystemProcess process;
        private readonly IContentPanel window;
        private readonly ITextConsole console;
        private readonly string[] args;

        public ISystemProcess Process => process;
        public string[] Arguments => args;
        public IContentPanel Window => window;
        public ITextConsole Console => console;
        
        public ProgramContext(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args)
        {
            this.process = process;
            this.window = window;
            this.console = console;
            this.args = args;
        }
    }
}