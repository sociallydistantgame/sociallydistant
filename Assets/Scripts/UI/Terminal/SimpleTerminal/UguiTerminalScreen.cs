#define USE_OLD_RENDERER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codice.CM.Common.Merge;
using Cysharp.Threading.Tasks;
using TMPro;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Data;
using UI.CustomGraphics;
using UI.Terminal.SimpleTerminal.Data;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.Terminal.SimpleTerminal
{
	public class UguiTerminalScreen : 
		MonoBehaviour, 
		IDrawableScreen
	{
		private readonly Dictionary<int, Color> colors = new Dictionary<int, Color>();
		private readonly StringBuilder stringBuilder = new StringBuilder();
		private readonly Dictionary<Color, string> hexColors = new Dictionary<Color, string>();

		private ColorCell[] colorCells = Array.Empty<ColorCell>();

		public Color fgColor;
		private bool hasFocus;
		private bool showCursor;
		private Color bgColor;
		private Texture2D? bgImage;
		private TrixelAudioSource trixelAudio;
		private SimpleTerminalRenderer term;
		private float characterWidth;
		private float lineHeight;
		private NewTextMeshProTerminalRenderer textRenderer;
		private bool plottersAreDirty = false;
		private int rowCount;
		private int columnCount;
		private bool textIsDirty;
		private int cursorX = -1;
		private int cursorY = -1;
		private LayoutElement layoutElement;
		private float cursorBlinkTimer;
		private float blinkTimer;
		private bool hideBlinking;

		[Header("Color Plotters")]
		[SerializeField]
		private RectanglePlotter backgroundColorPlotter = null!;
		
		[Header("Text")]
		[SerializeField]
		private TMP_FontAsset font;

		[SerializeField]
		private float cursorBlinkInterval = 0.5f;

		[SerializeField]
		private float blinkInterval = 0.75f;
		
		[SerializeField]
		private int fontSize;

		[SerializeField]
		private TextMeshProUGUI textMeshPro = null!;
		
		[Header("Sound Effects")]
		[SerializeField]
		private SoundEffectAsset asciiBeep = null!;

		[Header("Appearance")]
		[SerializeField]
		private bool showBackgroundColor;
		
		[Header("UI")]
		[SerializeField]
		private Graphic backgroundGraphic;
		
		public int DefaultBackgroundId => term.DefaultBackgroundId;
		public int DefaultForegroundId => term.DefaultForegroundId;
		
		public float LineHeight => lineHeight;
		public float CharacterWidth => characterWidth;
		public float UnscaledLineHeight { get; private set; }
		public float UnscaledCharacterWidth { get; private set; }

		private void Awake()
		{
			this.MustGetComponent(out trixelAudio);
			this.MustGetComponentInParent(out term);
			this.MustGetComponent(out layoutElement);
			this.ApplyFallbackPalette();

			textMeshPro.font = this.font;
			textMeshPro.fontSize = this.fontSize;
		}

		private void Start()
		{
			this.CalculateTextSize();
		}

		public bool Selected(int x, int y)
		{
			return term.Selected(x, y);
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
		}

		/// <inheritdoc />
		public void Resize(int columns, int rows)
		{
			// textRenderer?.Resize(columns, rows);
			
#if USE_OLD_RENDERER
			ReallocateTextPool(columns, rows);
#endif
		}
        
		private void ReallocateTextPool(int columns, int rows)
		{
			rowCount = rows;
			columnCount = columns;
			
            Array.Resize(ref colorCells, columns * rows);

            stringBuilder.EnsureCapacity(columns * rows * 8); // Ensures there's always enough room for an absurd amount of markup.

            if (layoutElement == null)
	            return;

            layoutElement.minWidth = columnCount * UnscaledCharacterWidth;
            layoutElement.minHeight = rowCount * UnscaledLineHeight;
            
            layoutElement.preferredWidth = layoutElement.minWidth;
            layoutElement.preferredHeight = layoutElement.minHeight;
		}

		public void SetColor(int index, Color c)
		{
			if (this.colors.ContainsKey(index))
				this.colors[index] = c;
			else
				this.colors.Add(index, c);
		}
		
		public void SetColor(int index, string c)
		{
			if (!ColorUtility.TryParseHtmlString(c, out Color color))
				throw new FormatException($"Couldn't parse the specified color as an HTML color: {c}");
			
			SetColor(index, color);
		}

		public Color GetColor(int index, Color rgbColor)
		{
			if (index == -1)
				return rgbColor;

			return colors[index];
		}
		
		public string GetColorOld(int c, Color rgbColor = default)
		{
			if (c == -1)
				return ColorUtility.ToHtmlStringRGB(rgbColor);

			if (this.colors.ContainsKey(c))
				return ColorUtility.ToHtmlStringRGB(colors[c]);
			return "#000000";
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

		/// <summary>
		///		Builds a lookup table translating Unity colors for the current palette to HTML color strings for use in TextMeshPro.
		/// </summary>
		private void BuildHexLookups()
		{
			this.hexColors.Clear();

			foreach (Color color in this.colors.Values.Where(color => !this.hexColors.ContainsKey(color)))
			{
				this.hexColors.Add(color, ColorUtility.ToHtmlStringRGB(color));
			}
		}

		private void UpdateBackgroundColor()
		{
			fgColor = this.colors[DefaultForegroundId];
			textMeshPro.color = fgColor;
			
			if (showBackgroundColor)
				bgColor = GetColor(DefaultBackgroundId, default);
			else
				bgColor = new Color(0, 0, 0, 0);
		}

		private void Update()
		{
			this.backgroundGraphic.color = bgColor;
			this.cursorBlinkTimer += Time.deltaTime;
			this.blinkTimer += Time.deltaTime;

			if (this.blinkTimer >= this.blinkInterval)
			{
				this.blinkTimer = 0;
				this.textIsDirty = true;
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
		}

		public void Bell()
		{
			if (!showCursor)
			{
				this.textIsDirty = true;
				this.plottersAreDirty = true;
			}
			
			this.showCursor = true;
			this.cursorBlinkTimer = 0;
			
			if (this.asciiBeep != null)
				this.trixelAudio.Play(this.asciiBeep);
		}

		/// <inheritdoc />
		public void ScreenPointToCell(SimpleTerminal term, float x, float y, out int column, out int row)
		{
			column = (int)Math.Floor(x / this.UnscaledCharacterWidth);
			row = (term.Rows - 1) - (int)Math.Floor(y / this.UnscaledLineHeight);
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

		private string LookupHexColor(Color color)
		{
			if (!hexColors.TryGetValue(color, out string hex))
			{
				hex = ColorUtility.ToHtmlStringRGB(color);
				this.hexColors.Add(color,hex);
			}

			return hex;
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
            
			this.textMeshPro.SetText(stringBuilder);
		}
		
		private void UpdateBackgroundCells()
		{
			var currentRow = 0;
			var currentColumn = 0;

			Rect rect = default;
			Color color = default;

			rect.height = lineHeight;

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
	                rect.y += lineHeight;
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
	                rect.x = cellColumn * characterWidth;
	                color = newColor;
                }

                rect.width += characterWidth;

                currentRow = cellRow;
                currentColumn = cellColumn;
			}

			backgroundColorPlotter.Refresh();
		}
        
		private void CalculateTextSize()
		{
			char ch = '#';
			var style = FontStyles.Normal;
			float fs = fontSize;
    
			// Compute scale of the target point size relative to the sampling point size of the font asset.
			float pointSizeScale = fs / (font.faceInfo.pointSize * font.faceInfo.scale);
			float emScale = fs * 0.01f;
            
            

			float styleSpacingAdjustment = (style & FontStyles.Bold) == FontStyles.Bold ? font.boldSpacing : 0;
			float normalSpacingAdjustment = font.normalSpacingOffset;

			// Make sure the given unicode exists in the font asset.
			font.TryAddCharacters(ch.ToString());
			if (!font.characterLookupTable.TryGetValue(ch, out TMP_Character character))
				character = font.characterLookupTable['?'];
			float width = (character.glyph.metrics.horizontalAdvance * pointSizeScale +
			               (styleSpacingAdjustment + normalSpacingAdjustment) * emScale);
			
			Vector3 scale = transform.lossyScale;

			float height = font.faceInfo.lineHeight
			         / font.faceInfo.pointSize
			         * fs;
			
			this.characterWidth = width / scale.x;
			this.lineHeight = height / scale.y;
			UnscaledLineHeight = height;
			UnscaledCharacterWidth = width;
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