using UnityEngine;

namespace AcidicGui.Widgets
{
	public class LabelWidget : IWidget
	{
		public string Text { get; set; } = string.Empty;
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			LabelWidgetController label = assembler.GetLabel(destination);
			label.Text = Text;
			return label;
		}
	}
}