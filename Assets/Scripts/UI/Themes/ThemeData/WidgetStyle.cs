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

		public SystemColorsData SystemColors => systemColors;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(systemColors, assets, nameof(systemColors));
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.PushDefaultSection("Widget styles", out _);
			builder.AddLabel("Use this section to customize the general look and feel of various UI widgets, as well as choose what colors to use for system colors.");
			builder.PopDefaultSection();
			
			systemColors.BuildWidgets(builder, markDirtyAction, editContext);
		}
	}
}