using System.Collections.Generic;
using AcidicGui.Widgets;
using UI.ScrollViews;
using UnityEngine;
using UnityExtensions;

namespace UI.Widgets
{
	public class WidgetList : ScrollViewController<WidgetListViewsHolder>
	{
		[SerializeField]
		private SystemWidgets systemWidgets = null!;

		private WidgetRecycleBin recycleBin;
		private ScrollViewItemList<IWidget> widgets;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.AssertAllFieldsAreSerialized(typeof(WidgetList));
			widgets = new ScrollViewItemList<IWidget>(this);

			recycleBin = this.systemWidgets.RecycleBin;
		}

		/// <inheritdoc />
		public void SetItems(IList<IWidget> widgetList)
		{
			this.widgets.SetItems(widgetList);
		}
		
		/// <inheritdoc />
		protected override WidgetListViewsHolder CreateModel(int itemIndex)
		{
			var vh = new WidgetListViewsHolder();
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(WidgetListViewsHolder newOrRecycled)
		{
			IWidget widget = widgets[newOrRecycled.ItemIndex];

			if (newOrRecycled.Widget != widget || newOrRecycled.WidgetController == null)
			{
				if (newOrRecycled.RecyclableWidget != null && newOrRecycled.WidgetController != null)
				{
					recycleBin.Recycle(newOrRecycled.WidgetController);

					newOrRecycled.WidgetController = null;
					newOrRecycled.RecyclableWidget = null;
				}
				
				newOrRecycled.WidgetController = widget.Build(this.systemWidgets, newOrRecycled.ViewRoot);
			}

            newOrRecycled.RecyclableWidget = newOrRecycled.WidgetController.gameObject;
            newOrRecycled.WidgetController = newOrRecycled.WidgetController;
            newOrRecycled.Widget = widget;

            newOrRecycled.WidgetController.UpdateUI();
            newOrRecycled.WidgetController.gameObject.SetActive(true);
		}

		/// <inheritdoc />
		protected override void OnBeforeDestroyViewsHolder(WidgetListViewsHolder inRecycleBinOrVisible)
		{
			base.OnBeforeDestroyViewsHolder(inRecycleBinOrVisible);

			if (inRecycleBinOrVisible.RecyclableWidget == null)
				return;

			if (inRecycleBinOrVisible.WidgetController == null)
				return;

			recycleBin.Recycle(inRecycleBinOrVisible.WidgetController);

			inRecycleBinOrVisible.WidgetController = null;
			inRecycleBinOrVisible.RecyclableWidget = null;
		}
	}
}