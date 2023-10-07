using System;
using UnityEngine;

namespace AcidicGui.Widgets
{
	public class ListItemWidget<TData> : IWidget
	{
		public ListWidget List { get; set; }
		public string? Title { get; set; }
		public TData Data { get; set; }
		public Action<TData>? Callback { get; set; }
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			if (this.List == null)
				throw new InvalidOperationException("Cannot build this ListItemWidget, because it is not assigned to a ListWidget.");

			if (this.List.ToggleGroup == null)
				throw new InvalidOperationException("Cannot build this ListItemWidget because the assigned ListWidget doesn't have a ToggleGroup yet. Ensure that the ListWidget is in the same section as the ListItemWidget, and that the ListWidget is added before any of its ListItemWidgets.");
			
			ListItemWidgetController controller = assembler.GetListItem(destination);

			controller.Title = this.Title;
			controller.Group = this.List.ToggleGroup;
			controller.Callback = OnSelect;
			
			return controller;
		}

		private void OnSelect()
		{
			Callback?.Invoke(this.Data);
		}
	}
}