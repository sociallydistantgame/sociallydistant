using UnityEngine;

namespace AcidicGui.Widgets
{
	public class SectionWidget : IWidget
	{
		public string Text { get; set; } = string.Empty;
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			SectionWidgetController section = assembler.GetSectionWidget(destination);
			section.Text = Text;
			return section;
		}
	}
}