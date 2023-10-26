#nullable enable
using ThisOtherThing.UI.Shapes;
using TMPro;
using UI.Theming;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	public class InputFieldCustomizer : SelectableShellElement
	{
		[SerializeField]
		private TextMeshProUGUI placeholder = null!;

		[SerializeField]
		private TextMeshProUGUI text = null!;

		[SerializeField]
		private TMP_InputField inputField = null!;

		[SerializeField]
		private Rectangle background = null!;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(InputFieldCustomizer));
			base.Awake();
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			// Selection highlight comes from the basic Selected graphic color in the theme.
			inputField.selectionColor = theme.GetGraphicColor(GraphicColor.Selected, Provider.UseDarkMode);
			
			// Text and placeholder colors
			text.color = theme.TranslateColor(theme.WidgetStyles.InputFieldTheme.TextColor, Provider.UseDarkMode);
			placeholder.color = theme.TranslateColor(theme.WidgetStyles.InputFieldTheme.PlaceholderColor, Provider.UseDarkMode);
			inputField.caretColor = text.color;
			
			// Apply the input field background style to the background rectangler
			// I'm so fucking stoned I just invented the sequel to rectangles
			
			if (IsPressed)
				background.ApplyRectangleStyle(theme, theme.WidgetStyles.InputFieldTheme.BackgroundPressed, Provider.UseDarkMode);
			else if (IsSelected)
				background.ApplyRectangleStyle(theme, theme.WidgetStyles.InputFieldTheme.BackgroundActive, Provider.UseDarkMode);
			else if (IsHovered)
				background.ApplyRectangleStyle(theme, theme.WidgetStyles.InputFieldTheme.BackgroundHover, Provider.UseDarkMode);
			else
				background.ApplyRectangleStyle(theme, theme.WidgetStyles.InputFieldTheme.Background, Provider.UseDarkMode);
			
			
		}
	}
}