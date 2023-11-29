using System;
using AcidicGui.Widgets;
using UI.Applications.Chat;
using UI.Themes.Serialization;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class ThemeColor : IThemeData
	{
		[SerializeField]
		private bool isCustom;

		[SerializeField]
		private string value = string.Empty;

		public bool IsCustom => isCustom;
		public string Data => value;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(ref isCustom, nameof(isCustom), true);
			serializer.Serialize(ref value, nameof(value), "#00000000");
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.AddWidget(new ThemeColorSelectWidget
			{
				ElementName = builder.Name ?? "Theme color",
				ElementDescription = builder.Description,
				IsCustomColor = isCustom,
				ColorData = value,
				EditContext = editContext,
				Callback = (newValue, custom) =>
				{
					isCustom = custom;
					value = newValue;
					markDirtyAction();
				}
			});
		}
		
		public static implicit operator ThemeColor(UnityEngine.Color color)
		{
			var themeColor = new ThemeColor();

			themeColor.isCustom = true;
			themeColor.value = ColorUtility.ToHtmlStringRGBA(color);
			
			return themeColor;
		}
	}
}