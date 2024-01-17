#nullable enable
using System;
using System.IO;
using OS.Devices;

namespace UI.Shell
{
	public class FileOutputConsole : 
		ITextConsole,
		IDisposable
	{
		private readonly ITextConsole input;
		private readonly Stream fileStream;
		private readonly StreamWriter fileWriter;
		
		/// <inheritdoc />
		public ConsoleInputData? ReadInput()
		{
			return input.ReadInput();
		}

		public FileOutputConsole(ITextConsole input, Stream output)
		{
			this.input = input;
			this.fileStream = output;

			this.fileWriter = new StreamWriter(fileStream);
		}
		
		public void Dispose()
		{
			fileWriter.Dispose();
			fileStream.Dispose();
		}

		/// <inheritdoc />
		public bool IsInteractive => input.IsInteractive;

		/// <inheritdoc />
		public void ClearScreen()
		{
			fileWriter.Flush();
			fileStream.Position = 0;
			fileStream.SetLength(0);
		}

		/// <inheritdoc />
		public void WriteText(string text)
		{
			fileWriter.Write(text);
		}
	}
}