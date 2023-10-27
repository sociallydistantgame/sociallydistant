#nullable enable
using TMPro;
using UI.Theming;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	public class DropdownItemCustomizer : ShellElement
	{
		[SerializeField]
		private TextMeshProUGUI label = null!;

		[SerializeField]
		private Graphic checkmark = null!;
		
		private Toggle toggle = null!;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			this.MustGetComponent(out toggle);
			base.Awake();
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			label.color = theme.TranslateColor(theme.WidgetStyles.DropdownTheme.LabelColor, Provider.UseDarkMode);
			checkmark.color = label.color;

			Color selectedColor = theme.GetGraphicColor(GraphicColor.Selected, Provider.UseDarkMode);

			toggle.colors = new ColorBlock
			{
				colorMultiplier = 1,
				fadeDuration = 0.1f,
				normalColor = default,
				highlightedColor = selectedColor * 0.75f,
				pressedColor = selectedColor * 0.5f,
				selectedColor = selectedColor,
				disabledColor = default
			};
		}
	}
}