using System;
using UnityEngine;

namespace AcidicGui.Widgets
{
	public class ListItemWidget<TData> : IWidget
	{
		public IWidget? Image { get; set; }
		
		public string? Title { get; set; }
		public string? Description { get; set; }
		public TData Data { get; set; }
		public Action<TData>? Callback { get; set; }
		public bool Selected { get; set; }
		
		/// <inheritdoc />
		public WidgetController Build(IWidgetAssembler assembler, RectTransform destination)
		{
			ListItemWidgetController controller = assembler.GetListItem(destination);

			controller.Title = this.Title;
			controller.Description = Description;
			controller.Callback = OnSelect;
			controller.Selected = Selected;

			controller.ImageWidget = this.Image?.Build(assembler, controller.ImageSlot);
			
			return controller;
		}

		private void OnSelect()
		{
			Callback?.Invoke(this.Data);
		}
	}
}