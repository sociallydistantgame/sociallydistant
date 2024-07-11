using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Scripting;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Terminal;

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
        var context = new OperatingSystemExecutionContext(this.Process);
        var shell = new InteractiveShell(context);
        
        shell.HandleExceptionsGracefully = true;
        shell.Setup(terminalWidget.Console);

        while (Process.IsAlive)
        {
            try
            {
                await shell.Run();
            }
            catch (ScriptEndException sex)
            {
                Process.Kill(sex.ExitCode);
            }
        }
    }
}