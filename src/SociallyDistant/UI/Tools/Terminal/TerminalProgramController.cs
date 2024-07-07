using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Programs;

namespace SociallyDistant.UI.Tools.Terminal;

public sealed class TerminalProgramController : ProgramController
{
    private readonly TextWidget text = new();
    
    private TerminalProgramController(ProgramContext context) : base(context)
    {
        context.Window.Content = text;

        text.WordWrapping = true;
        text.UseMarkup = true;
        text.HorizontalAlignment = HorizontalAlignment.Left;

        text.Padding = 12;
        text.MaximumSize = new Vector2(720, 0);
    }

    protected override void Main()
    {
        text.Text = $@"<size=20><color=#1baaf7><b>Soon, this will be the Terminal.</b></color></size>

For now, it is just a basic text label. However, the fact you got this far shows that a significant portion of Socially Distant has been ported to the new engine.

<b>To get to this point, several things needed to happen.</b>

First, the game engine needed to initialize. That's obvious.

Next, several assets needed to be loaded. This includes the fonts you're reading now.

A visual style for Acidic GUI needed to be set up. That's why the game looks like it does.

Speaking of UI, I had to <b>write</b> Acidic GUI so it could actually be used in the first place. Then I needed to implement a whole fuckton of widgets Socially Distant needs. One of them is TextWidget, which is what you're seeing now.

Then I needed to write dev tools. You can press the Tilde key to open the dev menu, and F9 to open the GUI inspector. They're both useful, and use IMGUI.

After that, I needed to port enough of the game to load a save file. World manager, the network simulation, the player computer, desktop, things like that.

So yeah. Here we are.

By the way, hello {User.UserName}@{User.Computer.Name}!";
    }
}