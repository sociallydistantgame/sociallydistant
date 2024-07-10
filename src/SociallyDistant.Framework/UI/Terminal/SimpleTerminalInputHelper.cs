using Microsoft.Xna.Framework.Input;

namespace SociallyDistant.Core.UI.Terminal;

public class SimpleTerminalInputHelper
{
    private readonly ITerminalSounds sounds;
    private readonly SimpleTerminal  term;
    private readonly Action<string>  Write;

    public SimpleTerminalInputHelper(ITerminalSounds sounds, SimpleTerminal term, Action<string> writeMethod)
    {
        this.sounds = sounds;
        this.term = term;
        this.Write = writeMethod;
    }

    public void Delete()
    {
        Write("\x7f");
    }

    public void UpArrow()
    {
        this.Write("\x1b[1A");
    }

    public void DownArrow()
    {
        this.Write("\x1b[1B");
    }

    public void RightArrow()
    {
        this.Write("\x1b[1C");
    }

    public void LeftArrow()
    {
        this.Write("\x1b[1D");
    }
		
    public void Backspace()
    {
        Char('\b');
    }

    public void Enter()
    {
        // We only write a CR, even on POSIX systems, since this maps to Enter in the line editor. Writing
        // the LF breaks line editor.
        this.Write("\r");
    }

    public void Char(char ch)
    {
        if (ch != '\r' && ch != '\n')
            sounds.PlayTypingSound();
			
        this.Write(ch.ToString());
    }

    public void Raw(Keys keyCode, bool modifierControl, bool modifierAlt, bool modifierShift)
    {
        // Fix modifiers for keystrokes where the main key is the same as the modifier
        if (keyCode == Keys.LeftControl || keyCode == Keys.RightControl)
            modifierControl = false;

        if (keyCode == Keys.LeftShift || keyCode == Keys.RightShift)
            modifierShift = false;

        if (keyCode == Keys.LeftAlt || keyCode == Keys.RightAlt)
            modifierAlt = false;
			
			
        var modifierMask = 0;

        if (modifierControl)
            modifierMask += 1;

        if (modifierAlt)
            modifierMask += 2;

        if (modifierShift)
            modifierMask += 4;

        if (modifierMask == 0)
        {
            switch (keyCode)
            {
                case Keys.Left:
                    this.LeftArrow();
                    return;
                case Keys.Right:
                    this.RightArrow();
                    return;
                case Keys.Up:
                    this.UpArrow();
                    return;
                case Keys.Down:
                    this.DownArrow();
                    return;
                case Keys.Back:
                    this.Backspace();
                    return;
                case Keys.Enter:
                    this.Enter();
                    return;
                case Keys.Delete:
                    this.Delete();
                    return;
            }
        }
			
        // Anything else is written as an escape sequence with a tilde terminator.
        this.Char('\x1b');
        this.Char('[');

        var rawKeyCode = (int) keyCode;
        this.Write(rawKeyCode.ToString());
        this.Write(";");
        this.Write(modifierMask.ToString());
			
        this.Write("~");
    }
}