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
		private SystemColorsData systemColors = new SystemColorsData();

		[SerializeField]
		private InputFieldTheme inputFieldTheme = new InputFieldTheme();
		
		public SystemColorsData SystemColors => systemColors;
		public InputFieldTheme InputFieldTheme => inputFieldTheme;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(systemColors, assets, nameof(systemColors));
			serializer.Serialize(inputFieldTheme, assets, nameof(inputFieldTheme));
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.PushDefaultSection("Widget styles", out _);
			builder.AddLabel("Use this section to customize the general look and feel of various UI widgets, as well as choose what colors to use for system colors.");
			builder.PopDefaultSection();
			
			systemColors.BuildWidgets(builder, markDirtyAction, editContext);
			
			inputFieldTheme.BuildWidgets(builder, markDirtyAction, editContext);
		}
	}
}