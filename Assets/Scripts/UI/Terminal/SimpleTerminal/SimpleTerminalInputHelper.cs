using System;

namespace UI.Terminal.SimpleTerminal
{
	public class SimpleTerminalInputHelper
	{
		private readonly Action<string> Write;

		public SimpleTerminalInputHelper(Action<string> writeMethod)
		{
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

		public void End()
		{
			this.Write("\x1b[4~");
		}

		public void Home()
		{
			this.Write("\x1b[1~");
		}

		public void Backspace()
		{
			this.Write("\b");
		}

		public void Enter()
		{
			// We only write a CR, even on POSIX systems, since this maps to Enter in the line editor. Writing
			// the LF breaks line editor.
			this.Write("\r");
		}

		public void Char(char ch)
		{
			this.Write(ch.ToString());
		}
	}

	public enum MouseButton
	{
		Left,
		Middle,
		Right
	}
}