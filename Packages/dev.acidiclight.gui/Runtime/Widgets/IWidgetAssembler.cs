using UnityEngine;

namespace AcidicGui.Widgets
{
	public interface IWidgetAssembler
	{
		WidgetRecycleBin RecycleBin { get; }
		
		SectionWidgetController GetSectionWidget(RectTransform destination);

		LabelWidgetController GetLabel(RectTransform destination);
	}
}