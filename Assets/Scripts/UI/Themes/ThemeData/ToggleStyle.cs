#nullable enable
using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UI.Widgets.Settings;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class ToggleStyle : IThemeData
	{
		[SerializeField]
		private ThemeGraphic checkmark = new ThemeGraphic();

		[SerializeField]
		private float width = 22;

		[SerializeField]
		private float height = 22;

		[SerializeField]
		private ThemeRectangleStyle background = new ThemeRectangleStyle();
		
		[SerializeField]
		private ThemeRectangleStyle backgroundHover = new ThemeRectangleStyle();
		
		[SerializeField]
		private ThemeRectangleStyle backgroundPressed = new ThemeRectangleStyle();

		[SerializeField]
		private ThemeRectangleStyle backgroundActive = new ThemeRectangleStyle();

		[SerializeField]
		private ThemeRectangleStyle backgroundChecked = new ThemeRectangleStyle();
		
		[SerializeField]
		private ThemeRectangleStyle backgroundHoverChecked = new ThemeRectangleStyle();
		
		[SerializeField]
		private ThemeRectangleStyle backgroundPressedChecked = new ThemeRectangleStyle();

		[SerializeField]
		private ThemeRectangleStyle backgroundActiveChecked = new ThemeRectangleStyle();
		
		public ThemeGraphic Checkmark => checkmark;
		public float Width => width;
		public float Height => height;
		public ThemeRectangleStyle Background => background;
		public ThemeRectangleStyle BackgroundHover => backgroundHover;
		public ThemeRectangleStyle BackgroundPressed => backgroundPressed;
		public ThemeRectangleStyle BackgroundActive => backgroundActive;
		public ThemeRectangleStyle BackgroundChecked => backgroundChecked;
		public ThemeRectangleStyle BackgroundHoverChecked => backgroundHoverChecked;
		public ThemeRectangleStyle BackgroundPressedChecked => backgroundPressedChecked;
		public ThemeRectangleStyle BackgroundActiveChecked => backgroundActiveChecked;

		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(ref width, nameof(width), 22);
			serializer.Serialize(ref height, nameof(height), 22);
			
			serializer.Serialize(checkmark, assets, nameof(checkmark));
			serializer.Serialize(background, assets, nameof(background));
			serializer.Serialize(backgroundHover, assets, nameof(backgroundHover));
			serializer.Serialize(backgroundPressed, assets, nameof(backgroundPressed));
			serializer.Serialize(backgroundActive, assets, nameof(backgroundActive));
			
			serializer.Serialize(backgroundChecked, assets, nameof(backgroundChecked));
			serializer.Serialize(backgroundHoverChecked, assets, nameof(backgroundHoverChecked));
			serializer.Serialize(backgroundPressedChecked, assets, nameof(backgroundPressedChecked));
			serializer.Serialize(backgroundActiveChecked, assets, nameof(backgroundActiveChecked));

		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			string? prefix = builder.Name;
			if (string.IsNullOrWhiteSpace(prefix))
				prefix = "Toggle widget";

			builder.PushDefaultSection(prefix, out _);

			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = $"{prefix} Width",
				CurrentValue = width.ToString(),
				Callback = (v) =>
				{
					if (!float.TryParse(v, out width))
						width = 0;
					markDirtyAction();
				}
			});
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = $"{prefix} Height",
				CurrentValue = height.ToString(),
				Callback = (v) =>
				{
					if (!float.TryParse(v, out height))
						height = 0;
					markDirtyAction();
				}
			});

			builder.Name = "Checkmark graphic";
			builder.Description = null;
			checkmark.BuildWidgets(builder, markDirtyAction, editContext);
			
			builder.PopDefaultSection();

			builder.PushDefaultSection($"{prefix} normal", out _);
			background.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
			
			builder.PushDefaultSection($"{prefix} while hovered", out _);
			backgroundHover.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
			
			builder.PushDefaultSection($"{prefix} while pressed", out _);
			backgroundPressed.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
			
			builder.PushDefaultSection($"{prefix} when focused", out _);
			backgroundActive.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
			
			builder.PushDefaultSection($"{prefix} normal (Checked)", out _);
			backgroundChecked.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
			
			builder.PushDefaultSection($"{prefix} while hovered (Checked)", out _);
			backgroundHoverChecked.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
			
			builder.PushDefaultSection($"{prefix} while pressed (Checked)", out _);
			backgroundPressedChecked.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
			
			builder.PushDefaultSection($"{prefix} when focused (Checked)", out _);
			backgroundActiveChecked.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
		}
	}
}