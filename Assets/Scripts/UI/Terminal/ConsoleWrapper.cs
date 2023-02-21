#nullable enable

using System;
using OS.Devices;

namespace UI.Terminal
{
	public class ConsoleWrapper
	{
		private readonly ITextConsole textConsole;

		public ConsoleWrapper(ITextConsole textConsole)
		{
			this.textConsole = textConsole;
		}

		public void Clear()
		{
			textConsole.ClearScreen();
		}

		public void Write(string text)
		{
			textConsole.WriteText(text);
		}

		public void WriteLine()
		{
			Write(Environment.NewLine);
		}

		public void WriteLine(string text)
		{
			Write(text);
			WriteLine();
		}

		public bool ReadLine(out string text)
		{
			return textConsole.TryDequeueSubmittedInput(out text);
		}
	}
}