using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Terminal;
using SociallyDistant.UI.Windowing;

namespace SociallyDistant.UI.Tools.Terminal;

public sealed class TerminalProgramController : ProgramController
{
    private readonly TerminalWidget terminalWidget = new();
    
    private TerminalProgramController(ProgramContext context) : base(context)
    {
        context.Window.Content = terminalWidget;
        context.Window.Window.SetWindowHints(new WindowHints { ClientRendersWindowBackground = true });

        // TODO: Bind this to Interface Preferences.
        terminalWidget.BackgroundOpacity = 0.75f;
    }

    protected override async void Main()
    {
        var shellProcess = Process.Fork();
        var context = new OperatingSystemExecutionContext(shellProcess);
        var shell = new InteractiveShell(context);
        
        shell.HandleExceptionsGracefully = true;
        shell.Setup(terminalWidget.Console);

        while (shellProcess.IsAlive)
        {
            try
            {
                await shell.Run();
            }
            catch (ScriptEndException sex)
            {
                shellProcess.Kill(sex.ExitCode);
            }
        }

        this.CloseWindow(shellProcess.ExitCode);
    }
}