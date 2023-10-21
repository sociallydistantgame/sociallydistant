#nullable enable
using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class SystemColorsData : IThemeData
	{
		[SerializeField]
		private ThemeColor background = new ThemeColor();

		[SerializeField]
		private ThemeColor text = new ThemeColor();

		[SerializeField]
		private ThemeColor selection = new ThemeColor();

		[SerializeField]
		private ThemeColor panel = new ThemeColor();

		[SerializeField]
		private ThemeColor toolbar = new ThemeColor();
		
		public ThemeColor Background => background;
		public ThemeColor Text => text;
		public ThemeColor Selection => selection;
		public ThemeColor Panel => panel;
		public ThemeColor ToolBar => toolbar;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(background, assets, nameof(background));
			serializer.Serialize(text, assets, nameof(text));
			serializer.Serialize(selection, assets, nameof(selection));
			serializer.Serialize(panel, assets, nameof(panel));
			serializer.Serialize(toolbar, assets, nameof(toolbar));
			
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.PushDefaultSection("System Colors", out _);
			builder.AddLabel("These colors are used across many of the UI widgets. It's recommended to use named colors for these, so that the colors can change depending on whether the user is in dark or light mode.");

			builder.Name = "Background";
			builder.Description = "The background of windows, documents, and most scroll views.";
			background.BuildWidgets(builder, markDirtyAction, editContext);

			builder.Name = "Text";
			builder.Description = "The color of most text elements and icons";
			text.BuildWidgets(builder, markDirtyAction, editContext);

			builder.Name = "Selections";
			builder.Description = "Highlight color for text selections in input fields.";
			selection.BuildWidgets(builder, markDirtyAction, editContext);
			
			builder.Name = "Panel";
			builder.Description = "The background color of sidebars and other panels";
			panel.BuildWidgets(builder, markDirtyAction, editContext);
			
			builder.Name = "Toolbar";
			builder.Description = "The background color of toolbars, menu bars, and tabbers.";
			toolbar.BuildWidgets(builder, markDirtyAction, editContext);
		}
	}
}