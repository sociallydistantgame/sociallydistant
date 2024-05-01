using System.Collections.Generic;
using UnityEngine;

namespace AcidicGui.Widgets
{
	public interface IWidget
	{
		WidgetController Build(IWidgetAssembler assembler, RectTransform destination);
	}
}