#nullable enable
using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class InputFieldTheme : IThemeData
	{
		[SerializeField]
		private ThemeColor textColor = new ThemeColor();

		[SerializeField]
		private ThemeColor placeholderColor = new ThemeColor();

		[SerializeField]
		private ThemeRectangleStyle background = new ThemeRectangleStyle();
		[SerializeField]
		private ThemeRectangleStyle backgroundHover = new ThemeRectangleStyle();
		[SerializeField]
		private ThemeRectangleStyle backgroundPressed = new ThemeRectangleStyle();
		[SerializeField]
		private ThemeRectangleStyle backgroundActive = new ThemeRectangleStyle();
		
		public ThemeColor TextColor => textColor;
		public ThemeColor PlaceholderColor => placeholderColor;
		public ThemeRectangleStyle Background => background;
		public ThemeRectangleStyle BackgroundHover => backgroundHover;
		public ThemeRectangleStyle BackgroundPressed => backgroundPressed;
		public ThemeRectangleStyle BackgroundActive => backgroundActive;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(textColor, assets, nameof(textColor));
			serializer.Serialize(placeholderColor, assets, nameof(placeholderColor));
			serializer.Serialize(background, assets, nameof(background));
			serializer.Serialize(backgroundHover, assets, nameof(backgroundHover));
			serializer.Serialize(backgroundActive, assets, nameof(backgroundActive));
			serializer.Serialize(backgroundPressed, assets, nameof(backgroundPressed));
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.PushDefaultSection("Input Fields", out _);

			builder.AddLabel("Customize the appearance of text fields and text areas.");

			builder.Name = "Text color";
			builder.Description = "Choose the color to use for input field text.";
            textColor.BuildWidgets(builder, markDirtyAction, editContext);
            
            builder.Name = "Placeholder color";
            builder.Description = "Choose the color to use for the placeholder text of an empty input field.";
            placeholderColor.BuildWidgets(builder, markDirtyAction, editContext);
            builder.PopDefaultSection();

            builder.Name = null;
            builder.Description = null;
            
            builder.PushDefaultSection("Input field - Normal", out _);
            
			background.BuildWidgets(builder, markDirtyAction, editContext);
            
			builder.PopDefaultSection();
			
			builder.PushDefaultSection("Input field - While hovered", out _);
            
			backgroundHover.BuildWidgets(builder, markDirtyAction, editContext);
            
			builder.PopDefaultSection();
			
			builder.PushDefaultSection("Input field - When pressed", out _);
            
			backgroundPressed.BuildWidgets(builder, markDirtyAction, editContext);
            
			builder.PopDefaultSection();
			
			builder.PushDefaultSection("Input field - While selected", out _);
            
			backgroundActive.BuildWidgets(builder, markDirtyAction, editContext);
            
			builder.PopDefaultSection();
		}
	}
}