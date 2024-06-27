using UnityEngine;
using UnityEngine.UI;

namespace AcidicGui.Widgets
{
	public class ListWidget : IWidget
	{
		public bool AllowSelectNone { get; set; }
		
		public ToggleGroup? ToggleGroup { get; set; }
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			ListWidgetController controller = assembler.GetList(destination);

			controller.AllowSelectNone = AllowSelectNone;

			ToggleGroup = controller.ToggleGroup;
			
			return controller;
		}
	}
}