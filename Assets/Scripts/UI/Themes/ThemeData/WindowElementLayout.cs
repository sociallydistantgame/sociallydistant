using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UI.Widgets.Settings;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class WindowElementLayout : IThemeData
	{
		[SerializeField]
		private Vector2 elementSize;

		[SerializeField]
		private Vector2 elementOffset;

		[SerializeField]
		private int anchorType;

		public WindowAnchor Anchor => (WindowAnchor) anchorType;
		public Vector2 Offset => elementOffset;
		public Vector2 Size => elementSize;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(ref elementOffset.x, "x", 0);
			serializer.Serialize(ref elementOffset.y, "y", 0);
			serializer.Serialize(ref elementSize.x, "width", 0);
			serializer.Serialize(ref elementSize.y, "height", 0);
            
			serializer.Serialize(ref anchorType, nameof(anchorType), (int) WindowAnchor.TopLeft);
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.AddWidget(new SettingsDropdownWidget
			{
				Title = "Anchor",
				Description = "Set which edge or corner of the titlebar that this element is positioned relative to.",
				CurrentIndex = anchorType,
				Choices = Enum.GetNames(typeof(WindowAnchor)),
				Callback = value =>
				{
					anchorType = value;
					markDirtyAction();
				}
			});

			builder.AddLabel("Offset");
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "X",
				CurrentValue = elementOffset.x.ToString(),
				Callback = str =>
				{
					if (!float.TryParse(str, out float value))
						return;

					elementOffset.x = value;
					markDirtyAction();
				}
			});
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Y",
				CurrentValue = elementOffset.y.ToString(),
				Callback = str =>
				{
					if (!float.TryParse(str, out float value))
						return;

					elementOffset.y = value;
					markDirtyAction();
				}
			});
			
			builder.AddLabel("Size");
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Width",
				CurrentValue = elementSize.x.ToString(),
				Callback = str =>
				{
					if (!float.TryParse(str, out float value))
						return;

					elementSize.x = value;
					markDirtyAction();
				}
			});
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Height",
				CurrentValue = elementSize.y.ToString(),
				Callback = str =>
				{
					if (!float.TryParse(str, out float value))
						return;

					elementSize.y = value;
					markDirtyAction();
				}
			});
			
			
		}
	}

	public enum WindowAnchor
	{
		TopLeft,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight
	}
}