#nullable enable
using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UI.Widgets.Settings;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class DropdownTheme : IThemeData
	{
		[SerializeField]
		private ThemeColor labelColor = new ThemeColor();

		[SerializeField]
		private ThemeGraphic arrowGraphic = new ThemeGraphic();

		[SerializeField]
		private ThemeRectangleStyle normalBackground = new ThemeRectangleStyle();

		[SerializeField]
		private ThemeRectangleStyle hoverBackground = new ThemeRectangleStyle();

		[SerializeField]
		private ThemeRectangleStyle pressedBackground = new ThemeRectangleStyle();

		[SerializeField]
		private ThemeRectangleStyle activeBackground = new ThemeRectangleStyle();

		[SerializeField]
		private ThemeMargins contentPadding = new ThemeMargins();

		[SerializeField]
		private float arrowContainerWidth;

		[SerializeField]
		private ThemeRectangleStyle contentBackground = new ThemeRectangleStyle();
		
		public ThemeColor LabelColor => labelColor;
		public ThemeGraphic ArrowGraphic => arrowGraphic;

		public ThemeRectangleStyle Background => normalBackground;
		public ThemeRectangleStyle BackgroundHover => hoverBackground;
		public ThemeRectangleStyle BackgroundPressed => pressedBackground;
		public ThemeRectangleStyle BackgroundActive => activeBackground;
		public ThemeMargins ContentPadding => contentPadding;
		public float ArrowContainerWidth => arrowContainerWidth;
		public ThemeRectangleStyle ContentBackground => contentBackground;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(labelColor, assets, nameof(labelColor));
			serializer.Serialize(arrowGraphic, assets, nameof(arrowGraphic));
			serializer.Serialize(normalBackground, assets, nameof(normalBackground));
			serializer.Serialize(hoverBackground, assets, nameof(hoverBackground));
			serializer.Serialize(pressedBackground, assets, nameof(pressedBackground));
			serializer.Serialize(activeBackground, assets, nameof(activeBackground));
			serializer.Serialize(contentPadding, assets, nameof(contentPadding));
			serializer.Serialize(ref arrowContainerWidth, nameof(arrowContainerWidth), 22);
			serializer.Serialize(contentBackground, assets, nameof(contentBackground));

		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.PushDefaultSection("Dropdowns", out _);

			builder.Name = "Label color";
			builder.Description = "Choose the color of the text displaying the current selection in a dropdown.";
			labelColor.BuildWidgets(builder, markDirtyAction, editContext);

			builder.PushDefaultSection("Dropdown content background", out _);
			contentBackground.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
			
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Arrow container width",
				CurrentValue = arrowContainerWidth.ToString(),
				Callback = (v) =>
				{
					if (!float.TryParse(v, out arrowContainerWidth))
						arrowContainerWidth = 0;
					markDirtyAction();
				}
			});
			
			builder.Name = "Arrow graphic";
			builder.Description = null;
			arrowGraphic.BuildWidgets(builder, markDirtyAction, editContext);

			builder.AddLabel("Dropdown content padding");
			
			contentPadding.BuildWidgets(builder, markDirtyAction, editContext);
			
			builder.PopDefaultSection();

			builder.Name = null;
			builder.Description = null;

			builder.PushDefaultSection("Dropdown normal", out _);

			normalBackground.BuildWidgets(builder, markDirtyAction, editContext);
			
			builder.PopDefaultSection();
			
			builder.PushDefaultSection("Dropdown while hovered", out _);

			hoverBackground.BuildWidgets(builder, markDirtyAction, editContext);
			
			builder.PopDefaultSection();
			
			builder.PushDefaultSection("Dropdown when pressed", out _);

			pressedBackground.BuildWidgets(builder, markDirtyAction, editContext);
			
			builder.PopDefaultSection();
			
			builder.PushDefaultSection("Dropdown when focused", out _);

			activeBackground.BuildWidgets(builder, markDirtyAction, editContext);
			
			builder.PopDefaultSection();
		}
	}
}