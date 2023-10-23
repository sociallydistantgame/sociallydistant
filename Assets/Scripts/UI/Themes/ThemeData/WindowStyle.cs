using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class WindowStyle : IThemeData
	{
		[SerializeField]
		private ThemeMargins windowBorderSizes = new ThemeMargins();

		[SerializeField]
		private ThemeGraphic activeBorderDecoration = new ThemeGraphic();
		
		[SerializeField]
		private ThemeGraphic inactiveBorderDecoration = new ThemeGraphic();

		[SerializeField]
		private WindowElementLayout iconLayout = new WindowElementLayout();
        
		[SerializeField]
		private WindowElementLayout titleTextLayout = new WindowElementLayout();
        
		[SerializeField]
		private WindowElementLayout closeButtonLayout = new WindowElementLayout();
        
		[SerializeField]
		private WindowElementLayout minimizeButtonLayout = new WindowElementLayout();
        
		[SerializeField]
		private WindowElementLayout maximizeButtonLayout = new WindowElementLayout();
		
		public ThemeMargins WindowBorderSizes => windowBorderSizes;
		public ThemeGraphic ActiveDecorations => activeBorderDecoration;
		public ThemeGraphic InactiveDecorations => inactiveBorderDecoration;

		public WindowElementLayout IconLayout => iconLayout;
		public WindowElementLayout TitleTextLayout => titleTextLayout;
		public WindowElementLayout CloseButtonLayout => closeButtonLayout;
		public WindowElementLayout MaximizeButtonLayout => maximizeButtonLayout;
		public WindowElementLayout MinimizeButtonLayout => minimizeButtonLayout;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(windowBorderSizes, assets, nameof(windowBorderSizes));
			serializer.Serialize(activeBorderDecoration, assets, nameof(activeBorderDecoration));
			serializer.Serialize(inactiveBorderDecoration, assets, nameof(inactiveBorderDecoration));
			
			serializer.Serialize(iconLayout, assets, nameof(iconLayout));
			serializer.Serialize(titleTextLayout, assets, nameof(titleTextLayout));
			serializer.Serialize(closeButtonLayout, assets, nameof(closeButtonLayout));
			serializer.Serialize(maximizeButtonLayout, assets, nameof(maximizeButtonLayout));
			serializer.Serialize(minimizeButtonLayout, assets, nameof(minimizeButtonLayout));
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.PushDefaultSection("Borders and Title", out _);
			builder.AddLabel("These settings affect the general appearance of the window's borders and title bar.");
			
			builder.Name = "Window border sizes";
			builder.Description = "This determines the size of each edge of the window.";
			windowBorderSizes.BuildWidgets(builder, markDirtyAction, editContext);

			builder.Name = "Active decorator";
			builder.Description = "A graphic that shows on the window when the window has focus.";
            activeBorderDecoration.BuildWidgets(builder, markDirtyAction, editContext);
            
            builder.Name = "Inactive decorator";
            builder.Description = "A graphic that shows on a window when the window doesn't have focus.";
            inactiveBorderDecoration.BuildWidgets(builder, markDirtyAction, editContext);

            builder.PopDefaultSection();

            builder.Name = string.Empty;
            builder.Description = string.Empty;
            
            builder.PushDefaultSection("Title Icon", out _);
            
            iconLayout.BuildWidgets(builder, markDirtyAction, editContext);
            
            builder.PopDefaultSection();
		}
	}
}