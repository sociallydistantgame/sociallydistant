#nullable enable

using AcidicGui.Widgets;
using UI.Shell.InfoPanel;
using UI.SystemSettings;
using UnityEngine;
using UnityExtensions;

namespace UI.Widgets
{
	public class WidgetListViewsHolder : AutoSizedItemsViewsHolder
	{
		private RecyclableCustomView customView;

		public RectTransform ViewRoot => customView.ViewsRect;
		
		public IWidget? Widget { get; set; }
		public GameObject? RecyclableWidget { get; set; }
		public WidgetController? WidgetController { get; set; }
		
		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponent(out customView);
			base.CollectViews();
		}
	}
}