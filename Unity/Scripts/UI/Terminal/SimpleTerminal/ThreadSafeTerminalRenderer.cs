using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using OS.Network.MessageTransport;
using TMPro;
using UI.CustomGraphics;
using UI.Terminal.SimpleTerminal.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Terminal.SimpleTerminal
{
	/// <summary>
	///		Implementation of the <see cref="IDrawableScreen"/> allowing the Socially Distant Terminal emulator
	///		to interface with Unity and TextMeshPro in a thread-safe way.
	/// </summary>
	public sealed class ThreadSafeTerminalRenderer : IDrawableScreen
	{
		private readonly ConcurrentDictionary<Color, string> hexColors = new ConcurrentDictionary<Color, string>();
		private readonly LayoutElement layoutElement;
		private readonly SimpleTerminalRenderer owningRenderer;
		private readonly WorkQueue workQueue;
		private readonly StringBuilder stringBuilder = new StringBuilder();
		private readonly ConcurrentDictionary<int, Color> colors = new ConcurrentDictionary<int, Color>();
		private readonly TextMeshProUGUI text;
		private readonly RectanglePlotter backgroundColorPlotter;
        
		private ColorCell[] colorCells = Array.Empty<ColorCell>();
		private int rowCount;
		private int columnCount;
		private int cursorX;
		private int cursorY;
		private bool textIsDirty = false;
		private bool plottersAreDirty = false;
		private bool showCursor;
		private float cursorBlinkTime;
		private bool hasFocus;
		private Color fgColor;
		private Color bgColor;
		private bool hideBlinking;
		private bool showBackgroundColor;
		private float cursorBlinkTimer;
		private float blinkTimer;
		private float blinkInterval = 0.25f;
		private float cursorBlinkInterval = 0.5f;

		private volatile bool hasBlinkingElements = false;
		private volatile bool needsLayoutUpdate = true;
		private volatile bool textPresentNeeded = true;
		private volatile bool backgroundPresentNeeded = true;

		public int DefaultBackgroundId => SimpleTerminal.defaultbg;
		public int DefaultForegroundId => SimpleTerminal.defaultfg;
		
		public ThreadSafeTerminalRenderer(
			SimpleTerminalRenderer owningRenderer, 
			WorkQueue workQueue,
			LayoutElement layoutElement,
			TextMeshProUGUI text,
			RectanglePlotter backgroundColorPlotter)
		{
			this.owningRenderer = owningRenderer;
			this.workQueue = workQueue;
			this.layoutElement = layoutElement;
			this.text = text;
			this.backgroundColorPlotter = backgroundColorPlotter;
			
			this.ApplyFallbackPalette();
		}
		
		public void ApplyFallbackPalette()
		{
			this.colors.Clear();

			// Fallback palette is based on the default palette in st
			this.SetColor(0, "#000000");
			this.SetColor(1, "#ce0000");
			this.SetColor(2, "#00ce00");
			this.SetColor(3, "#cece00");
			this.SetColor(4, "#0000ee");
			this.SetColor(5, "#ce00ce");
			this.SetColor(6, "#00cece");
			this.SetColor(7, "#e6e6e6");

			this.SetColor(8, "#808080");
			this.SetColor(9, "#ff0000");
			this.SetColor(10, "#00ff00");
			this.SetColor(11, "#ffff00");
			this.SetColor(12, "#5c5cff");
			this.SetColor(13, "#ff00ff");
			this.SetColor(14, "#00ffff");
			this.SetColor(15, "#ffffff");

			this.SetColor(256, "#cccccc");
			this.SetColor(257, "#555555");
			this.SetColor(258, "#000000");
			this.SetColor(259, "#e6e6e6");
			
			this.UpdateBackgroundColor();
			this.BuildHexLookups();
		}
		
		private void UpdateBackgroundColor()
		{
			fgColor = this.colors[DefaultForegroundId];

			if (showBackgroundColor)
				bgColor = GetColor(DefaultBackgroundId, default);
			else
				bgColor = new Color(0, 0, 0, 0);
		}
		
		/// <summary>
		///		Builds a lookup table translating Unity colors for the current palette to HTML color strings for use in TextMeshPro.
		/// </summary>
		private void BuildHexLookups()
		{
			this.hexColors.Clear();

			foreach (Color color in this.colors.Values.Where(color => !this.hexColors.ContainsKey(color)))
			{
				this.hexColors[color] = ColorUtility.ToHtmlStringRGB(color);
			}
		}
		
		public Color GetColor(int index, Color rgbColor)
		{
			if (index == -1)
				return rgbColor;

			return colors[index];
		}
		
		/// <inheritdoc />
		public void DrawLine(SimpleTerminal term, ref Glyph[] glyphs, int x1, int y, int x2)
		{
			for (int x = x1; x < x2; x++)
			{
				ref Glyph glyph = ref glyphs[x];

				bool reverse = (glyph.mode & GlyphAttribute.ATTR_REVERSE) != 0;

				if (term.Selected(x, y))
					reverse = !reverse;

				Color foregroundColor = GetColor(glyph.fg, glyph.fgRgb);
				Color backgroundColor;

				if (!reverse && glyph.bg == DefaultBackgroundId)
				{
					backgroundColor = new Color(0, 0, 0, 0);
				}
				else
				{
					backgroundColor = GetColor(glyph.bg, glyph.bgRgb);
				}

				if (reverse)
				{
					(backgroundColor, foregroundColor) = (foregroundColor, backgroundColor);
				}

				if ((glyph.mode & GlyphAttribute.ATTR_FAINT) != 0)
				{
					foregroundColor.a *= 0.5f;
					backgroundColor.a *= 0.5f;
				}

				int cellIndex = (y * term.Columns) + x;

				ref ColorCell cell = ref colorCells[cellIndex];
				if (cell.Foreground != foregroundColor)
				{
					cell.Foreground = foregroundColor;
					textIsDirty = true;
				}

				if (cell.Background != backgroundColor)
				{
					cell.Background = backgroundColor;
					plottersAreDirty = true;
				}

				bool blink = (glyph.mode & GlyphAttribute.ATTR_BLINK) != 0;
				bool bold = (glyph.mode & GlyphAttribute.ATTR_BOLD) != 0;
				bool italic = (glyph.mode & GlyphAttribute.ATTR_ITALIC) != 0;
				bool underline = (glyph.mode & GlyphAttribute.ATTR_UNDERLINE) != 0;
				bool strikethrough = (glyph.mode & GlyphAttribute.ATTR_STRUCK) != 0;

				if (glyph.character != cell.Character
				    || bold != cell.Bold
				    || italic != cell.Italic
				    || underline != cell.Underline
				    || strikethrough != cell.Strikethrough
				    || blink != cell.Blinking)
				{
					textIsDirty = true;

					cell.Bold = bold;
					cell.Italic = italic;
					cell.Underline = underline;
					cell.Strikethrough = strikethrough;
					cell.Blinking = blink;

					cell.Character = glyph.character;
				}
			}

			CheckForBlinks();
		}

		private void CheckForBlinks()
		{
			this.hasBlinkingElements = false;
			
			for (var i = 0; i < colorCells.Length; i++)
			{
				ref ColorCell cell = ref colorCells[i];
				if (cell.Blinking)
				{
					hasBlinkingElements = true;
					break;
				}
			}
		}

		/// <inheritdoc />
		public void Resize(int columns, int rows)
		{
			rowCount = rows;
			columnCount = columns;

			Array.Resize(ref colorCells, columns * rows);

			stringBuilder.EnsureCapacity(columns * rows * 8); // Ensures there's always enough room for an absurd amount of markup.
		}

		/// <inheritdoc />
		public void Bell()
		{
			this.showCursor = true;
			this.cursorBlinkTimer = 0;
            
			// We can't call TrixelAudio APIs from the terminal render thread!
			workQueue.Enqueue(owningRenderer.Bell);
		}

		/// <inheritdoc />
		public void ScreenPointToCell(SimpleTerminal term, float x, float y, out int column, out int row)
		{
			column = (int)Math.Floor(x / owningRenderer.UnscaledCharacterWidth);
			row = (rowCount - 1) - (int)Math.Floor(y / owningRenderer.UnscaledLineHeight);
		}

		/// <inheritdoc />
		public void AfterRender(int cursorX, int cursorY)
		{
			if (this.cursorX != cursorX || this.cursorY != cursorY)
			{
				this.showCursor = true;
				this.cursorBlinkTimer = 0;
				this.cursorX = cursorX;
				this.cursorY = cursorY;
				this.textIsDirty = true;
				this.plottersAreDirty = true;
			}

			if (textIsDirty)
			{
				UpdateText();
				textIsDirty = false;
			}

			if (!plottersAreDirty)
				return;

			UpdateBackgroundCells();

			plottersAreDirty = false;
		}

		private void UpdateBackgroundCells()
		{
			var currentRow = 0;
			var currentColumn = 0;

			Rect rect = default;
			Color color = default;

			rect.height = owningRenderer.LineHeight;

			backgroundColorPlotter.Clear();

			for (var i = 0; i <= colorCells.Length; i++)
			{
				// End of cells
				if (i == colorCells.Length)
				{
					backgroundColorPlotter.Plot(rect, color);
					break;
				}

				ref ColorCell currentCell = ref colorCells[i];

				int cellColumn = i % columnCount;
				int cellRow = i / columnCount;

				// End of a row.
				if (cellRow > currentRow)
				{
					backgroundColorPlotter.Plot(rect, color);
					rect.width = 0;
					rect.y += owningRenderer.LineHeight;
					rect.x = 0;
				}

				bool isCursor = cellRow == cursorY && cellColumn == cursorX
				                                   && hasFocus && showCursor;

				Color newColor = isCursor ? currentCell.Foreground : currentCell.Background;

				// Color has changed.
				if (newColor != color)
				{
					backgroundColorPlotter.Plot(rect, color);
					rect.width = 0;
					rect.x = cellColumn * owningRenderer.CharacterWidth;
					color = newColor;
				}

				rect.width += owningRenderer.CharacterWidth;

				currentRow = cellRow;
				currentColumn = cellColumn;
			}

			backgroundPresentNeeded = true;
		}
		
		private void UpdateText()
		{
			var bold = false;
			var italic = false;
			var underline = false;
			var strikethrough = false;

			var currentRow = 0;
			var collectedWhitespace = 0;

			this.stringBuilder.Length = 0;

			Color color = fgColor;

			for (var i = 0; i < colorCells.Length; i++)
			{
				ref ColorCell cell = ref colorCells[i];

				int cellRow = i / columnCount;
				int cellColumn = i % columnCount;

				// End of a row.
				if (cellRow > currentRow)
				{
					collectedWhitespace = 0;
					currentRow = cellRow;
					this.stringBuilder.AppendLine();
				}

				bool isCursor = cellRow == cursorY && cellColumn == cursorX
				                                   && hasFocus && showCursor;
				Color newColor = isCursor ? cell.Background : cell.Foreground;

				bool isBold = cell.Bold;
				bool isItalic = cell.Italic;
				bool isUnderline = cell.Underline;
				bool isStrikethrough = cell.Strikethrough;

				if (isBold != bold)
					this.stringBuilder.Append(isBold ? "<b>" : "</b>");

				if (isItalic != italic)
					this.stringBuilder.Append(isItalic ? "<i>" : "</i>");

				if (isUnderline != underline)
					this.stringBuilder.Append(isUnderline ? "<u>" : "</u>");

				if (isStrikethrough != strikethrough)
					this.stringBuilder.Append(isStrikethrough ? "<s>" : "</s>");

				bool isWhitespace = char.IsWhiteSpace(cell.Character);

				if (color != newColor)
				{
					if (color != fgColor)
						stringBuilder.Append("</color>");

					if (newColor == fgColor)
						this.stringBuilder.Append("</color>");
					else
					{
						string hex = LookupHexColor(newColor);

						this.stringBuilder.Append("<color=#");
						this.stringBuilder.Append(hex);
						this.stringBuilder.Append('>');
					}

					color = newColor;
				}

				if (isWhitespace || (cell.Blinking && hideBlinking))
					collectedWhitespace++;
				else
				{
					int start = this.stringBuilder.Length;
					this.stringBuilder.Length += collectedWhitespace;

					for (var j = 0; j < collectedWhitespace; j++)
						stringBuilder[start + j] = ' ';

					collectedWhitespace = 0;

					stringBuilder.Append(cell.Character);
				}

				bold = isBold;
				italic = isItalic;
				underline = isUnderline;
				strikethrough = isStrikethrough;

				if (i == colorCells.Length - 1 && color != fgColor)
				{
					stringBuilder.Append("</color>");
				}
			}

			this.textPresentNeeded = true;
		}
		
		public void SetColor(int index, string c)
		{
			if (!ColorUtility.TryParseHtmlString(c, out Color color))
				throw new FormatException($"Couldn't parse the specified color as an HTML color: {c}");

			SetColor(index, color);
		}
		
		public void SetColor(int index, Color c)
		{
			this.colors[index] = c;
		}
		
		private string LookupHexColor(Color color)
		{
			if (!hexColors.TryGetValue(color, out string hex))
			{
				hex = ColorUtility.ToHtmlStringRGB(color);
				this.hexColors[color] = hex;
			}

			return hex;
		}
		
		/// <inheritdoc />
		public void SetFocus(bool isFocused)
		{
			if (this.hasFocus == isFocused)
				return;

			this.hasFocus = isFocused;
			this.textIsDirty = true;
			this.plottersAreDirty = true;
			this.cursorBlinkTimer = 0;
			this.showCursor = isFocused;
		}

		public void Present()
		{
			this.cursorBlinkTimer += Time.deltaTime;
			this.blinkTimer += Time.deltaTime;

			if (this.blinkTimer >= this.blinkInterval)
			{
				this.blinkTimer = 0;
				this.textIsDirty |= this.hasBlinkingElements;
				this.hideBlinking = !this.hideBlinking;
			}

			if (this.cursorBlinkTimer >= cursorBlinkInterval)
			{
				this.cursorBlinkTimer = 0;
				this.showCursor = !this.showCursor;

				// For performance reasons, don't actually refresh the blinking cursor
				// when we don't have focus. This is because the cursor never renders when we
				// aren't focused.
				if (this.hasFocus)
				{
					this.textIsDirty = true;
					this.plottersAreDirty = true;
				}
			}
			
			UpdateLayout();
			PresentBackground();
			PresentText();
		}

		private void PresentBackground()
		{
			if (!backgroundPresentNeeded)
				return;

			backgroundPresentNeeded = false;
			backgroundColorPlotter.Refresh();
		}
		
		private void PresentText()
		{
			if (!textPresentNeeded)
				return;

			text.color = fgColor;
			textPresentNeeded = false;
			this.text.SetText(this.stringBuilder);
		}
		
		private void UpdateLayout()
		{
			if (!needsLayoutUpdate)
				return;
			
			if (layoutElement == null)
				return;

			needsLayoutUpdate = false;
			layoutElement.minWidth = columnCount * owningRenderer.UnscaledCharacterWidth;
			layoutElement.minHeight = rowCount * owningRenderer.UnscaledLineHeight;
			layoutElement.preferredWidth = layoutElement.minWidth;
			layoutElement.preferredHeight = layoutElement.minHeight;
		}
		
		private struct ColorCell
		{
			public Color Foreground;
			public Color Background;

			public char Character;

			public bool Bold;
			public bool Italic;
			public bool Underline;
			public bool Strikethrough;
			public bool Blinking;
		}
	}
}