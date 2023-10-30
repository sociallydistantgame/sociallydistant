#nullable enable
using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UI.Widgets;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class WidgetStyle : IThemeData
	{
		[SerializeField]
		private ThemeTypography typography = new ThemeTypography();
		
		[SerializeField]
		private SystemColorsData systemColors = new SystemColorsData();

		[SerializeField]
		private InputFieldTheme inputFieldTheme = new InputFieldTheme();

		[SerializeField]
		private DropdownTheme dropdownTheme = new DropdownTheme();

		[SerializeField]
		private ToggleStyle toggleStyle = new ToggleStyle();

		[SerializeField]
		private ToggleStyle radioButtonStyle = new ToggleStyle();
		
		public SystemColorsData SystemColors => systemColors;
		public InputFieldTheme InputFieldTheme => inputFieldTheme;
		public DropdownTheme DropdownTheme => dropdownTheme;
		public ToggleStyle ToggleStyle => toggleStyle;
		public ToggleStyle RadioButtonStyle => radioButtonStyle;
		public ThemeTypography Typography => typography;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(typography, assets, nameof(typography));
			serializer.Serialize(systemColors, assets, nameof(systemColors));
			serializer.Serialize(inputFieldTheme, assets, nameof(inputFieldTheme));
			serializer.Serialize(dropdownTheme, assets, nameof(dropdownTheme));
			serializer.Serialize(toggleStyle, assets, nameof(toggleStyle));
			serializer.Serialize(radioButtonStyle, assets, nameof(radioButtonStyle));
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.PushDefaultSection("Widget styles", out _);
			builder.AddLabel("Use this section to customize the general look and feel of various UI widgets, as well as choose what colors to use for system colors.");
			builder.PopDefaultSection();
			
			systemColors.BuildWidgets(builder, markDirtyAction, editContext);
			inputFieldTheme.BuildWidgets(builder, markDirtyAction, editContext);
			dropdownTheme.BuildWidgets(builder, markDirtyAction, editContext);
			toggleStyle.BuildWidgets(builder, markDirtyAction, editContext);
			radioButtonStyle.BuildWidgets(builder, markDirtyAction, editContext);
		}
	}
}