#nullable enable
using System.Text;
using OS.Devices;

namespace GamePlatform
{
	public class StringConsole : ITextConsole
	{
		private readonly StringBuilder stringBuilder = new();
		
		/// <inheritdoc />
		public string WindowTitle { get; set; } = string.Empty;

		/// <inheritdoc />
		public bool IsInteractive => false;

		public string GetText()
		{
			return stringBuilder.ToString();
		}
		
		/// <inheritdoc />
		public void ClearScreen()
		{
			stringBuilder.Length = 0;
		}

		/// <inheritdoc />
		public void WriteText(string text)
		{
			stringBuilder.Append(text);
		}

		/// <inheritdoc />
		public ConsoleInputData? ReadInput()
		{
			return null;
		}
	}
}