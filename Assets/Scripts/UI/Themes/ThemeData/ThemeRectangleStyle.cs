#nullable enable
using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UI.Widgets.Settings;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class ThemeRectangleStyle : IThemeData
	{
		[SerializeField]
		private ThemeColor mainColor = new ThemeColor();

		[SerializeField]
		private ThemeColor borderColor = new ThemeColor();
		
		[SerializeField]
		private float borderThickness;

		[SerializeField]
		private float rounding = 0;

		public ThemeColor MainColor => mainColor;
		public ThemeColor BorderColor => borderColor;
		public float BorderThickness => borderThickness;
		public float Rounding => rounding;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(mainColor, assets, nameof(mainColor));
			serializer.Serialize(borderColor, assets, nameof(borderColor));
			serializer.Serialize(ref borderThickness, nameof(borderThickness), 0);
			serializer.Serialize(ref rounding, nameof(rounding), 0);
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			if (string.IsNullOrWhiteSpace(builder.CurrentSectionName))
				builder.PushDefaultSection("Widget style properties", out _);
			else
				builder.PushDefaultSection($"{builder.CurrentSectionName} style properties", out _);

			builder.Name = "Background color";
			builder.Description = null;
			mainColor.BuildWidgets(builder, markDirtyAction, editContext);
			builder.Name = "Border color";
			builder.Description = null;
			borderColor.BuildWidgets(builder, markDirtyAction, editContext);

			builder.Name = null;
			
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Border thickness",
				CurrentValue = borderThickness.ToString(),
				Callback = (v) =>
				{
					if (!float.TryParse(v, out borderThickness))
						borderThickness = 0;
					markDirtyAction();
				}
			});
			
			builder.AddWidget(new SettingsInputFieldWidget
			{
				Title = "Corner radius",
				CurrentValue = rounding.ToString(),
				Callback = (v) =>
				{
					if (!float.TryParse(v, out rounding))
						rounding = 0;
					markDirtyAction();
				}
			});
			
			builder.PopDefaultSection();
		}
	}
}