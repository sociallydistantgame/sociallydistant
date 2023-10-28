#nullable enable
using System;
using ThisOtherThing.UI.Shapes;
using UI.Themes.ThemeData;
using UI.Theming;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Themes.ThemedElements
{
	public class ToggleCustomizer : SelectableShellElement
	{
		[SerializeField]
		private Toggle toggle = null!;

		[SerializeField]
		private Image checkmark = null!;

		[SerializeField]
		private Rectangle background = null!;

		private LayoutElement layoutElement = null!;
		private bool isChecked;
		private bool isInGroup;

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ToggleCustomizer));
			this.MustGetComponent(out layoutElement);
			base.Awake();
		}

		private void Update()
		{
			bool wasChecked = isChecked;
			bool wasInGroup = isInGroup;

			isChecked = toggle.isOn;
			isInGroup = toggle.group != null;
			
			if (wasChecked != isChecked || wasInGroup != isInGroup)
				NotifyThemePropertyChanged();
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			// Get correct style. We use radio button styles if we're in a ToggleGroup.
			ToggleStyle style = isInGroup
				? theme.WidgetStyles.RadioButtonStyle
				: theme.WidgetStyles.ToggleStyle;
			
			// Size
			layoutElement.minWidth = style.Width;
			layoutElement.minHeight = style.Height;
			layoutElement.preferredWidth = layoutElement.minWidth;
			layoutElement.preferredHeight = layoutElement.minHeight;
			
			// Checkmark graphic
			checkmark.ApplyThemeGraphic(theme, style.Checkmark, Provider.UseDarkMode);
			
			// Background depends on whether we're checked
			if (isChecked)
			{
				if (IsPressed)
					background.ApplyRectangleStyle(theme, style.BackgroundPressedChecked, Provider.UseDarkMode);
				else if (IsSelected)
					background.ApplyRectangleStyle(theme, style.BackgroundActiveChecked, Provider.UseDarkMode);
				else if (IsHovered)
					background.ApplyRectangleStyle(theme, style.BackgroundHoverChecked, Provider.UseDarkMode);
				else
					background.ApplyRectangleStyle(theme, style.BackgroundChecked, Provider.UseDarkMode);
			}
			else
			{
				if (IsPressed)
					background.ApplyRectangleStyle(theme, style.BackgroundPressed, Provider.UseDarkMode);
				else if (IsSelected)
					background.ApplyRectangleStyle(theme, style.BackgroundActive, Provider.UseDarkMode);
				else if (IsHovered)
					background.ApplyRectangleStyle(theme, style.BackgroundHover, Provider.UseDarkMode);
				else
					background.ApplyRectangleStyle(theme, style.Background, Provider.UseDarkMode);
			}
		}
	}
}