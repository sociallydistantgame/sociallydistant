﻿using System.Collections.Generic;
using AcidicGui.Widgets;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using UnityEngine;
using UnityExtensions;

namespace UI.Widgets
{
	public class WidgetList : OSA<BaseParamsWithPrefab, WidgetListViewsHolder>
	{
		[SerializeField]
		private SystemWidgets systemWidgets = null!;

		private WidgetRecycleBin recycleBin;
		private SimpleDataHelper<IWidget> widgets;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.AssertAllFieldsAreSerialized(typeof(WidgetList));
			widgets = new SimpleDataHelper<IWidget>(this);

			recycleBin = this.systemWidgets.RecycleBin;
		}

		/// <inheritdoc />
		public void SetItems(IList<IWidget> widgetList)
		{
			if (!IsInitialized)
				Init();
			
			this.widgets.ResetItems(widgetList);
		}
		
		/// <inheritdoc />
		protected override WidgetListViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new WidgetListViewsHolder();

			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(WidgetListViewsHolder newOrRecycled)
		{
			IWidget widget = widgets[newOrRecycled.ItemIndex];

            WidgetController controller = widget.Build(this.systemWidgets, newOrRecycled.ViewRoot);

            newOrRecycled.RecyclableWidget = controller.gameObject;
            newOrRecycled.WidgetController = controller;

            controller.UpdateUI();
            
			ScheduleComputeVisibilityTwinPass();
		}

		/// <inheritdoc />
		protected override void OnBeforeRecycleOrDisableViewsHolder(WidgetListViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);

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