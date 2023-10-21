using AcidicGui.Widgets;
using UI.Themes.Serialization;
using UI.Widgets;
using UnityEngine;

namespace UI.Applications.Chat
{
	public class NamedColorEntry : IWidget
	{
		public IThemeEditContext? EditContext { get; set; }
		public string ColorName { get; set; } = string.Empty;
		public Color LightColor { get; set; }
		public Color DarkColor { get; set; }
		public bool IsTemporary { get; set; }
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			NamedColorEntryController controller = (assembler as SystemWidgets)!.GetNamedColorEntry(destination);

			controller.EditContext = EditContext;
			controller.ColorName = ColorName;
			controller.DarkColor = DarkColor;
			controller.LightColor = LightColor;
			controller.IsTemporary = IsTemporary;
			
			return controller;
		}
	}
}