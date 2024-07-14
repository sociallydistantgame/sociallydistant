using AcidicGUI.Widgets;
using SociallyDistant.Core.Programs;

namespace SociallyDistant.UI.Tools.Email;

public sealed class EmailProgramController : ProgramController
{
    private readonly TextWidget text = new();
    
    private EmailProgramController(ProgramContext context) : base(context)
    {
        context.Window.Title = "Email";
        context.Window.Content = text;

        text.Padding = 25;
    }

    protected override void Main()
    {
        text.Text = $"Hello, {User.UserName}@{User.Computer.Name}!";
    }
}