using UnityEngine;

namespace AcidicGui.Widgets
{
	public class LabelWidget : IWidget
	{
		public string Text { get; set; } = string.Empty;
		public bool AllowHttpLinks { get; set; } = false;
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			LabelWidgetController label = assembler.GetLabel(destination);
			label.Text = Text;
			label.AllowHttpLinks = AllowHttpLinks;
			return label;
		}
	}
}