#nullable enable
using System.Collections.Generic;
using Shell.InfoPanel;
using UI.ScrollViews;

namespace UI.Shell.InfoPanel
{
	public class InfoWidgetsController : ScrollViewController<InfoWidgetViewsHolder>
	{
		private ScrollViewItemList<InfoWidgetData> widgets;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			widgets = new ScrollViewItemList<InfoWidgetData>(this);
		}

		public void SetItems(IList<InfoWidgetData> widgetData)
		{
			this.widgets.SetItems(widgetData);
		}

		/// <inheritdoc />
		protected override InfoWidgetViewsHolder CreateModel(int itemIndex)
		{
			var viewsHolder = new InfoWidgetViewsHolder();
			return viewsHolder;
		}

		/// <inheritdoc />
		protected override void UpdateModel(InfoWidgetViewsHolder newOrRecycled)
		{
			InfoWidgetData data = this.widgets[newOrRecycled.ItemIndex];

			newOrRecycled.SetInfo(data);
		}
	}
}