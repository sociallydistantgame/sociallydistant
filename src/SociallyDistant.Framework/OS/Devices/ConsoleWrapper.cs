﻿#nullable enable

namespace SociallyDistant.Core.OS.Devices
{
	public class ConsoleWrapper
	{
		private readonly ITextConsole textConsole;
		private readonly LineEditor lineEditor;

		public ITextConsole Device => textConsole;
		public bool IsInteractive => textConsole.IsInteractive;

		public void SetWindowTitle(string windowTitle)
		{
			this.textConsole.WindowTitle = windowTitle;
		}

		public void ResetWindowTitle()
		{
			this.textConsole.WindowTitle = string.Empty;
		}
		
		public ConsoleWrapper(ITextConsole textConsole)
		{
			this.textConsole = textConsole;
			this.lineEditor =  new LineEditor(textConsole);
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
			return lineEditor.Update(out text);
		}

		public async Task<string> ReadLineAsync(IAutoCompleteSource? autoCompleteSource = null)
		{
			lineEditor.UsePasswordChars = false;
			lineEditor.AutoCompleteSource = autoCompleteSource;
			return await lineEditor.ReadLineAsync();
		}
		
		public async Task<string> ReadPasswordAsync()
		{
			lineEditor.UsePasswordChars = true;
			return await lineEditor.ReadLineAsync();
		}
		
		public ConsoleInputData? ReadKey()
		{
			return textConsole.ReadInput();
		}

		public async Task<ConsoleInputData> ReadKeyAsync()
		{
			ConsoleInputData? data = ReadKey();

			while (!data.HasValue)
			{
				await Task.Yield();
				data = ReadKey();
			}

			return data.Value;
		}
	}
}