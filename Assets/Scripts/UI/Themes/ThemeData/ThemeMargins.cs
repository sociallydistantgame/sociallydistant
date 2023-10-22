using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UI.Widgets.Settings;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class ThemeMargins : IThemeData
	{
		public int left;
		public int top;
		public int right;
		public int bottom;


		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(ref top, nameof(top), 0);
			serializer.Serialize(ref left, nameof(left), 0);
			serializer.Serialize(ref bottom, nameof(bottom), 0);
			serializer.Serialize(ref right, nameof(right), 0);
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Left",
				CurrentValue = left.ToString(),
				Callback = (v) =>
				{
					if (!int.TryParse(v, out left))
						left = 0;
					markDirtyAction();
				}
			});
			
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Top",
				CurrentValue = top.ToString(),
				Callback = (v) =>
				{
					if (!int.TryParse(v, out top))
						top = 0;
					markDirtyAction();
				}
			});
			
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Right",
				CurrentValue = right.ToString(),
				Callback = (v) =>
				{
					if (!int.TryParse(v, out right))
						right = 0;
					markDirtyAction();
				}
			});
			
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Bottom",
				CurrentValue = bottom.ToString(),
				Callback = (v) =>
				{
					if (!int.TryParse(v, out bottom))
						bottom = 0;
					markDirtyAction();
				}
			});
			
			
		}
		
		public static implicit operator RectOffset(ThemeMargins margins)
		{
			return new RectOffset(margins.left, margins.right, margins.top, margins.bottom);
		}
	}
}