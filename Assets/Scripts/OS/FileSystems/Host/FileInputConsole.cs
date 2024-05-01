#nullable enable
using System;
using System.IO;
using OS.Devices;
using UnityEngine.InputSystem.LowLevel;

namespace OS.FileSystems.Host
{
	public class FileInputConsole : 
		ITextConsole,
		IDisposable
	{
		private readonly ITextConsole output;
		private readonly Stream fileStream;
		private readonly StreamReader fileReader;
		
		/// <inheritdoc />
		public ConsoleInputData? ReadInput()
		{
			if (fileReader.EndOfStream)
				return null;

			int nextChar = fileReader.Read();
			if (nextChar <= 0)
				return null;

			return new ConsoleInputData((char) nextChar);
		}

		public FileInputConsole(ITextConsole output, Stream fileStream)
		{
			this.output = output;
			this.fileStream = fileStream;
			this.fileReader = new StreamReader(this.fileStream);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			fileReader.Dispose();
			fileStream.Dispose();

			if (output is IDisposable disposable)
				disposable.Dispose();
		}

		/// <inheritdoc />
		public string WindowTitle
		{
			get => output.WindowTitle;
			set => output.WindowTitle = value;
		}

		/// <inheritdoc />
		public bool IsInteractive => false;

		/// <inheritdoc />
		public void ClearScreen()
		{
			output.ClearScreen();
		}

		/// <inheritdoc />
		public void WriteText(string text)
		{
			output.WriteText(text);
		}

		/// <inheritdoc />
		public bool TryDequeueSubmittedInput(out string input)
		{
			input = string.Empty;
			if (fileReader.EndOfStream)
				return false;

			string? nextLine = fileReader.ReadLine();
			if (nextLine == null)
				return false;
			
			input = nextLine;
			return true;
		}
	}
}