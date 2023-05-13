#nullable enable

using System;
using OS.Devices;

namespace UI.Terminal
{
	public class ConsoleWrapper
	{
		private readonly ITextConsole textConsole;

		public bool SuppressInput
		{
			get => textConsole.SuppressInput;
			set => textConsole.SuppressInput = value;
		}
		
		public ConsoleWrapper(ITextConsole textConsole)
		{
			this.textConsole = textConsole;
		}

		public void Clear()
		{
			textConsole.ClearScreen();
		}

		public string Normalize(string text)
		{
			return text.Replace("\r\n", "\n")
				.Replace("\n\r", "\n")
				.Replace("\r", "\n")
				.Replace("\n", Environment.NewLine);
		}
		
		public void Write(string text)
		{
			textConsole.WriteText(Normalize(text));
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