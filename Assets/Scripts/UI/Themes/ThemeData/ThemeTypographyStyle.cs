using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UI.Widgets.Settings;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class ThemeTypographyStyle : IThemeData
	{
		[SerializeField]
		private int fontType;

		[SerializeField]
		private int casing;
		
		[SerializeField]
		private float fontSize = 12;

		[SerializeField]
		private ThemeColor textColor = new ThemeColor();

		[SerializeField]
		private bool bold;

		[SerializeField]
		private bool italic;

		[SerializeField]
		private bool underline;

		[SerializeField]
		private bool strikethrough;

		[SerializeField]
		private float characterSpacing = 0;

		[SerializeField]
		private float lineSpacing = 1;
		
		public bool Bold => bold;
		public bool Italic => italic;
		public bool Underline => underline;
		public bool Strikethrough => strikethrough;
		public ThemeColor TextColor => textColor;
		public float FontSize => fontSize;
		public ThemeFont Font => (ThemeFont) fontType;
		public TextCasing Casing => (TextCasing) casing;
		public float CharacterSpacing => characterSpacing;
		public float LineSpacing => lineSpacing;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(ref bold, nameof(bold), false);
			serializer.Serialize(ref italic, nameof(italic), false);
			serializer.Serialize(ref underline, nameof(underline), false);
			serializer.Serialize(ref strikethrough, nameof(strikethrough), false);
			serializer.Serialize(ref fontSize, nameof(fontSize), 12);
			serializer.Serialize(ref fontType, nameof(fontType), (int) ThemeFont.SansSerif);
			serializer.Serialize(ref casing, nameof(casing), (int) TextCasing.Normal);
			serializer.Serialize(ref characterSpacing, nameof(characterSpacing), 0);
			serializer.Serialize(ref lineSpacing, nameof(lineSpacing), 0);
			
			
			serializer.Serialize(textColor, assets, nameof(textColor));
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			BuildWidgets(builder, markDirtyAction, editContext, true);
		}
		
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext, bool showTextColorField)
		{
			builder.AddWidget(new SettingsDropdownWidget
			{
				Title = "Font style",
				Choices = Enum.GetNames(typeof(ThemeFont)),
				CurrentIndex = fontType,
				Callback = (choice) =>
				{
					fontType = choice;
					markDirtyAction();
				}
			});
			
			builder.AddWidget(new SettingsDropdownWidget
			{
				Title = "Casing",
				Description = "Change how text is capitalized",
				Choices = Enum.GetNames(typeof(TextCasing)),
				CurrentIndex = casing,
				Callback = (choice) =>
				{
					casing = choice;
					markDirtyAction();
				}
			});
			
			builder.Name = "Text color";
			builder.Description = null;
			
			if (showTextColorField)
				textColor.BuildWidgets(builder, markDirtyAction, editContext);

			builder.Name = null;

			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Font size",
				CurrentValue = fontSize.ToString(),
				Callback = (value) =>
				{
					if (float.TryParse(value, out float fs))
						fontSize = fs;
					markDirtyAction();
				}
			});
			
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Character spacing",
				CurrentValue = characterSpacing.ToString(),
				Callback = (value) =>
				{
					if (float.TryParse(value, out float fs))
						characterSpacing = fs;
					markDirtyAction();
				}
			});
			
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Line spacing",
				CurrentValue = lineSpacing.ToString(),
				Callback = (value) =>
				{
					if (float.TryParse(value, out float fs))
						lineSpacing = fs;
					markDirtyAction();
				}
			});

			builder.AddWidget(new SettingsToggleWidget
			{
				Title = "Bold",
				CurrentValue = bold,
				Callback = (v) =>
				{
					bold = v;
					markDirtyAction();
				}
			});
			
			builder.AddWidget(new SettingsToggleWidget
			{
				Title = "Italic",
				CurrentValue = italic,
				Callback = (v) =>
				{
					italic = v;
					markDirtyAction();
				}
			});
			
			builder.AddWidget(new SettingsToggleWidget
			{
				Title = "Underlined",
				CurrentValue = underline,
				Callback = (v) =>
				{
					underline = v;
					markDirtyAction();
				}
			});
			
			builder.AddWidget(new SettingsToggleWidget
			{
				Title = "Strikethrough",
				CurrentValue = strikethrough,
				Callback = (v) =>
				{
					strikethrough = v;
					markDirtyAction();
				}
			});
			
			
		}
	}
}