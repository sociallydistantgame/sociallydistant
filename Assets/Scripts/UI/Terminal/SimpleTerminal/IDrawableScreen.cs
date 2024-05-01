using UI.Terminal.SimpleTerminal.Data;

namespace UI.Terminal.SimpleTerminal
{
	public interface IDrawableScreen
	{
		void DrawLine(SimpleTerminal term, ref Glyph[] glyphs, int x1, int y, int x2);

		void Resize(int columns, int rows);

		void Bell();

		void ScreenPointToCell(SimpleTerminal term, float x, float y, out int column, out int row);

		void AfterRender(int cursorX, int cursorY);
		void SetFocus(bool isFocused);
	}
}