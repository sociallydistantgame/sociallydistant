using OS.Devices;

namespace Core.Scripting.Consoles
{
	public class RedirectedConsole : ITextConsole
	{
		private readonly ITextConsole input;
		private readonly ITextConsole output;
		
		/// <inheritdoc />
		public ConsoleInputData? ReadInput()
		{
			return input.ReadInput();
		}

		public RedirectedConsole(ITextConsole input, ITextConsole output)
		{
			this.input = input;
			this.output = output;
		}

		/// <inheritdoc />
		public string WindowTitle
		{
			get => output.WindowTitle;
			set => output.WindowTitle = value;
		}

		/// <inheritdoc />
		public bool IsInteractive => input.IsInteractive;

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
	}
}