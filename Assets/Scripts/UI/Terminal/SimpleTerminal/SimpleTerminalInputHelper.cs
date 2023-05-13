using System;

namespace UI.Terminal.SimpleTerminal
{
	public class SimpleTerminalInputHelper
	{
		private readonly SimpleTerminal term;
		private readonly Action<string> Write;

		public SimpleTerminalInputHelper(SimpleTerminal term, Action<string> writeMethod)
		{
			this.term = term;
			this.Write = writeMethod;
		}

		public void Delete()
		{
			if (term.SuppressInput)
				return;
			
			Write("\x7f");
		}

		public void UpArrow()
		{
			if (term.SuppressInput)
				return;

			this.Write("\x1b[1A");
		}

		public void DownArrow()
		{
			if (term.SuppressInput)
				return;

			this.Write("\x1b[1B");
		}

		public void RightArrow()
		{
			if (term.SuppressInput)
				return;

			this.Write("\x1b[1C");
		}

		public void LeftArrow()
		{
			if (term.SuppressInput)
				return;

			this.Write("\x1b[1D");
		}

		public void End()
		{
			if (term.SuppressInput)
				return;

			this.Write("\x1b[4~");
		}

		public void Home()
		{
			if (term.SuppressInput)
				return;

			this.Write("\x1b[1~");
		}

		public void Backspace()
		{
			if (term.SuppressInput)
				return;

			this.Write("\b");
		}

		public void Enter()
		{
			if (term.SuppressInput)
				return;

			// We only write a CR, even on POSIX systems, since this maps to Enter in the line editor. Writing
			// the LF breaks line editor.
			this.Write("\r");
		}

		public void Char(char ch)
		{
			if (term.SuppressInput)
				return;

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