using System.Text;
using OS.Devices;

namespace UI.Shell
{
	public class LineListConsole : ITextConsole
	{
		private readonly StringBuilder buffer = new StringBuilder();
		private int readPosition = 0;
		
		/// <inheritdoc />
		public void ClearScreen()
		{
			buffer.Length = 0;
			readPosition = 0;
		}

		/// <inheritdoc />
		public void WriteText(string text)
		{
			buffer.Append(text);
		}

		/// <inheritdoc />
		public bool TryDequeueSubmittedInput(out string input)
		{
			input = string.Empty;

			if (readPosition == buffer.Length)
				return false;

			input = ReadUntil('\n');
			return true;
		}

		private string ReadUntil(char character)
		{
			var sb = new StringBuilder();

			while (readPosition < buffer.Length)
			{
				char current = buffer[readPosition];
				if (character == current)
					break;

				sb.Append(current);
				readPosition++;
			}
			
			return sb.ToString();
		}
	}
}