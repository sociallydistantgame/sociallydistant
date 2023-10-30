#nullable enable
using System;
using TMPro;
using UI.Themes.ThemeData;
using UI.Theming;
using UnityEngine;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	public class TextStyleUpdater : ShellElement
	{
		[SerializeField]
		private bool ignoreColor;
		
		[SerializeField]
		private TextStyle textStyle = TextStyle.Paragraph;

		private TextMeshProUGUI text = null!;
		
		public TextStyle TextStyle
		{
			get => textStyle;
			set
			{
				textStyle = value;
				NotifyThemePropertyChanged();
			}
		}

		/// <inheritdoc />
		protected override void Awake()
		{
			this.MustGetComponent(out text);
			base.Awake();
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			ThemeTypographyStyle style = MapTextStyle(theme);

			var fontStyles = FontStyles.Normal;

			switch (style.Casing)
			{
				case TextCasing.Uppercase:
					fontStyles |= FontStyles.UpperCase;
					break;
				case TextCasing.Lowercase:
					fontStyles |= FontStyles.LowerCase;
					break;
                case TextCasing.Smallcaps:
	                fontStyles |= FontStyles.SmallCaps;
	                break;
			}

			if (style.Bold)
				fontStyles |= FontStyles.Bold;
			
			if (style.Italic)
				fontStyles |= FontStyles.Italic;
			
			if (style.Underline)
				fontStyles |= FontStyles.Underline;
			
			if (style.Strikethrough)
				fontStyles |= FontStyles.Strikethrough;
			
			
			
			text.fontStyle = fontStyles;
			
			text.font = Provider.GetFont(style.Font);
			text.fontSize = style.FontSize;
			
			if (!ignoreColor)
				text.color = theme.TranslateColor(style.TextColor, Provider.UseDarkMode);

			text.characterSpacing = style.CharacterSpacing;
			text.lineSpacing = style.LineSpacing;
		}

		private ThemeTypographyStyle MapTextStyle(OperatingSystemTheme theme)
		{
			ThemeTypography typography = theme.WidgetStyles.Typography;
			return this.textStyle switch
			{
				TextStyle.Paragraph => typography.Paragraph,
				TextStyle.Large => typography.Large,
				TextStyle.Small => typography.Small,
				TextStyle.Heading1 => typography.Heading1,
				TextStyle.Heading2 => typography.Heading2,
				TextStyle.Heading3 => typography.Heading3,
				TextStyle.Heading4 => typography.Heading4,
				TextStyle.Heading5 => typography.Heading5,
				TextStyle.Heading6 => typography.Heading6,
				TextStyle.Title => typography.Title,
				TextStyle.SectionTitle => typography.SectionTitle,
				TextStyle.Code => typography.Code,
				TextStyle.WindowTitle => theme.WindowDecorations.TitleTextFontStyle,
				_ => typography.Paragraph
			};
		}
	}
}