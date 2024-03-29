#nullable enable

namespace OS.Devices
{
	public interface ITextConsole
	{
		string WindowTitle { get; set; }
		bool IsInteractive { get; }
		
		void ClearScreen();
		void WriteText(string text);
		ConsoleInputData? ReadInput();
	}

	public sealed class NullConsole : ITextConsole
	{
		/// <inheritdoc />
		public string WindowTitle { get; set; }

		/// <inheritdoc />
		public bool IsInteractive => false;

		/// <inheritdoc />
		public void ClearScreen()
		{
		}

		/// <inheritdoc />
		public void WriteText(string text)
		{
		}

		/// <inheritdoc />
		public ConsoleInputData? ReadInput()
		{
			return null;
		}
	}
}