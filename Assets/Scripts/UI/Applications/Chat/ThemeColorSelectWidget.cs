using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UI.Widgets;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

namespace UI.Applications.Chat
{
	public class ThemeColorSelectWidget : IWidget
	{
		public string ElementName { get; set; } = string.Empty;
		public string ElementDescription { get; set; } = string.Empty;
		public IThemeEditContext? EditContext { get; set; }
		public bool IsCustomColor { get; set; }
		public string ColorData { get; set; } = string.Empty;
		public Action<string, bool>? Callback { get; set; }

		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			ThemeColorSelectWidgetController controller = (assembler as SystemWidgets)!.GetThemeColorSelect(destination);

			controller.ElementName = ElementName;
			controller.ElementDescription = ElementDescription;
			controller.EditContext = EditContext;
			controller.IsCustomColor = IsCustomColor;
			controller.ColorData = ColorData;
			controller.Callback = Callback;

			return controller;
		}
	}
}