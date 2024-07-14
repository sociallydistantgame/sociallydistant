using AcidicGUI.Widgets;
using SociallyDistant.Core.Programs;

namespace SociallyDistant.UI.Tools.WebBrowser;

public sealed class WebBrowserController : ProgramController
{
    private readonly TextWidget text = new();
    
    private WebBrowserController(ProgramContext context) : base(context)
    {
        context.Window.Content = text;

        text.Padding = 25;
    }

    protected override void Main()
    {
        text.Text = $"Hello, {Process.User.UserName}@{Process.User.Computer.Name}!";
    }
}