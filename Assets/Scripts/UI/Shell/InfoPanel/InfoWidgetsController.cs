#nullable enable
using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

namespace UI.Shell.InfoPanel
{
	public class InfoWidgetsController : OSA<BaseParamsWithPrefab, InfoWidgetViewsHolder>
	{
		private SimpleDataHelper<InfoWidgetData> widgets;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			widgets = new SimpleDataHelper<InfoWidgetData>(this);
		}

		public void SetItems(IList<InfoWidgetData> widgetData)
		{
			this.widgets.ResetItems(widgetData);
		}

		/// <inheritdoc />
		protected override InfoWidgetViewsHolder CreateViewsHolder(int itemIndex)
		{
			var viewsHolder = new InfoWidgetViewsHolder();
			
			viewsHolder.Init(_Params.ItemPrefab, _Params.Content, itemIndex, false);

			return viewsHolder;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(InfoWidgetViewsHolder newOrRecycled)
		{
			InfoWidgetData data = this.widgets[newOrRecycled.ItemIndex];

			newOrRecycled.SetInfo(data);
		}
	}
}