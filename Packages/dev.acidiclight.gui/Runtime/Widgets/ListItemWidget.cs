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
            
			ListItemWidgetController controller = assembler.GetListItem(destination);

			controller.Title = this.Title;
			controller.List = this.List;
			controller.Callback = OnSelect;
			
			return controller;
		}

		private void OnSelect()
		{
			Callback?.Invoke(this.Data);
		}
	}
}