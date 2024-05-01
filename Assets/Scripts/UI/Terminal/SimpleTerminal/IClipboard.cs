namespace UI.Terminal.SimpleTerminal
{
	public interface IClipboard
	{
		string GetText();

		void SetText(string text);
	}
}