using OS.Devices;

namespace UI.Shell
{
	public class RedirectedConsole : ITextConsole
	{
		private readonly ITextConsole input;
		private readonly ITextConsole output;

		public bool SuppressInput
		{
			get => input.SuppressInput && output.SuppressInput;
			set
			{
				this.input.SuppressInput = value;
				this.output.SuppressInput = value;
			}
		}
		
		public RedirectedConsole(ITextConsole input, ITextConsole output)
		{
			this.input = input;
			this.output = output;
		}

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
			return this.input.TryDequeueSubmittedInput(out input);
		}
	}
}