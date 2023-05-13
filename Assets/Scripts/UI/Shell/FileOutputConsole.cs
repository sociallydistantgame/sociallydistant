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

		public bool SuppressInput
		{
			get => input.SuppressInput;
			set => input.SuppressInput = value;
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

		/// <inheritdoc />
		public bool TryDequeueSubmittedInput(out string input)
		{
			return this.input.TryDequeueSubmittedInput(out input);
		}
	}
}