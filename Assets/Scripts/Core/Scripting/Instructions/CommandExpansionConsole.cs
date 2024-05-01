#nullable enable
using System.Text;
using OS.Devices;

namespace Core.Scripting.Instructions
{
	public sealed class CommandExpansionConsole : ITextConsole
	{
		private readonly ITextConsole underlyingConsole;
		private readonly StringBuilder text = new StringBuilder();


		/// <inheritdoc />
		public string WindowTitle
		{
			get => underlyingConsole.WindowTitle;
			set => underlyingConsole.WindowTitle = value;
		}

		/// <inheritdoc />
		public bool IsInteractive => false;

		public string Text => text.ToString().TrimEnd();
		
		public CommandExpansionConsole(ITextConsole underlyingConsole)
		{
			this.underlyingConsole = underlyingConsole;
		}
		
		/// <inheritdoc />
		public void ClearScreen()
		{
			text.Length = 0;
		}

		/// <inheritdoc />
		public void WriteText(string text)
		{
			this.text.Append(text);
		}

		/// <inheritdoc />
		public ConsoleInputData? ReadInput()
		{
			return underlyingConsole.ReadInput();
		}
	}
}