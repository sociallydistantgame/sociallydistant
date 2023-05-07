using System;
using System.Collections.Generic;
using TMPro;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Data;
using UI.Terminal.SimpleTerminal.Data;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Terminal.SimpleTerminal
{
	public class UguiTerminalScreen : MonoBehaviour, IDrawableScreen
	{
		private readonly Dictionary<int, string> colors = new Dictionary<int, string>();

		private Color bgColor;
		private Texture2D? bgImage;
		private TrixelAudioSource trixelAudio;
		private SimpleTerminalRenderer term;
		private float characterWidth;
		private float lineHeight;
        
		private RenderedLine[] renderedLines = Array.Empty<RenderedLine>();

		[Header("Text")]
		[SerializeField]
		private TMP_FontAsset font;

		[SerializeField]
		private int fontSize;

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

		public float LineHeight => lineHeight;
		public float CharacterWidth => characterWidth;
		
		
		private void Awake()
		{
			this.MustGetComponent(out trixelAudio);
			this.MustGetComponentInParent(out term);
			this.CalculateTextSize();
			this.ApplyFallbackPalette();
		}

		public bool Selected(int x, int y)
		{
			return term.Selected(x, y);
		}

		/// <inheritdoc />
		public void DrawLine(ref Glyph[] glyphs, int x1, int y, int x2)
		{
			RenderedLine line = this.renderedLines[y];
			line.SetGlyphs(ref glyphs, x1, x2, y);   
		}

		/// <inheritdoc />
		public void Resize(int columns, int rows)
		{
			ReallocateTextPool(columns, rows);
		}
        
		private void ReallocateTextPool(int columns, int rows)
		{
			if (this.renderedLines is null) this.renderedLines = new RenderedLine[rows];

			if (this.renderedLines.Length != rows)
			{
				for (int i = rows; i < this.renderedLines.Length; i++)
				{
					RenderedLine line = this.renderedLines[i];
					line.Destroy();
					this.renderedLines[i] = null;
				}

				SimpleTerminal.XRealloc(ref this.renderedLines, rows);

				for (var i = 0; i < this.renderedLines.Length; i++)
				{
					RenderedLine line = this.renderedLines[i];
					if (line is null)
						line = new RenderedLine(this);

					line.SetFont(font, fontSize);
					line.SetColumnSize(columns);
					this.renderedLines[i] = line;
				}
			}
		}
        
		public void SetColor(int index, string c)
		{
			if (c.StartsWith("#"))
				c = c.Remove(0, 1);

			if (this.colors.ContainsKey(index))
				this.colors[index] = c;
			else
				this.colors.Add(index, c);
		}
        
		public string GetColor(int c, Color rgbColor = default)
		{
			if (c == -1)
				return ColorUtility.ToHtmlStringRGB(rgbColor);

			if (this.colors.ContainsKey(c))
				return this.colors[c];
			return "#000000";
		}
        
		public void ApplyFallbackPalette()
		{
			this.colors.Clear();

			// Fallback palette is based on the default palette in st
			this.SetColor(0, "000000");
			this.SetColor(1, "ce0000");
			this.SetColor(2, "00ce00");
			this.SetColor(3, "cece00");
			this.SetColor(4, "0000ee");
			this.SetColor(5, "ce00ce");
			this.SetColor(6, "00cece");
			this.SetColor(7, "e6e6e6");

			this.SetColor(8, "#808080");
			this.SetColor(9, "ff0000");
			this.SetColor(10, "00ff00");
			this.SetColor(11, "ffff00");
			this.SetColor(12, "5c5cff");
			this.SetColor(13, "ff00ff");
			this.SetColor(14, "00ffff");
			this.SetColor(15, "ffffff");

			this.SetColor(256, "cccccc");
			this.SetColor(257, "555555");
			this.SetColor(258, "000000");
			this.SetColor(259, "e6e6e6");

			this.UpdateBackgroundColor();
		}

		private void UpdateBackgroundColor()
		{
			string bg = "#" + this.GetColor(DefaultBackgroundId);

			if (!ColorUtility.TryParseHtmlString(bg, out bgColor) || !showBackgroundColor)
				bgColor = new Color(0, 0, 0, 0);
		}

		private void Update()
		{
			this.backgroundGraphic.color = bgColor;
		}

		public void Bell()
		{
			if (this.asciiBeep != null)
				this.trixelAudio.Play(this.asciiBeep);
		}

		/// <inheritdoc />
		public void ScreenPointToCell(SimpleTerminal term, float x, float y, out int column, out int row)
		{
			column = (int)Math.Floor(x / this.characterWidth);
			row = (term.Rows - 1) - (int)Math.Floor(y / this.lineHeight);
		}

		private void CalculateTextSize()
		{
			char ch = '#';
			var style = FontStyles.Normal;
			float fs = fontSize;
    
			// Compute scale of the target point size relative to the sampling point size of the font asset.
			float pointSizeScale = fs / (font.faceInfo.pointSize * font.faceInfo.scale);
			float emScale = fs * 0.01f;

			float height = ((font.faceInfo.lineHeight - font.faceInfo.underlineOffset) * emScale) /
			               this.transform.lossyScale.y;
            
            

			float styleSpacingAdjustment = (style & FontStyles.Bold) == FontStyles.Bold ? font.boldSpacing : 0;
			float normalSpacingAdjustment = font.normalSpacingOffset;

			// Make sure the given unicode exists in the font asset.
			font.TryAddCharacters(ch.ToString());
			if (!font.characterLookupTable.TryGetValue(ch, out TMP_Character character))
				character = font.characterLookupTable['?'];
			float width = (character.glyph.metrics.horizontalAdvance * pointSizeScale +
			               (styleSpacingAdjustment + normalSpacingAdjustment) * emScale)
			              / transform.lossyScale.x;

			this.characterWidth = width;
			this.lineHeight = height;
		}
	}
}