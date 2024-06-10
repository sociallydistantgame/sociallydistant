using System;
using System.Collections.Generic;
using Architecture;
using UI.ScrollViews;
using UniRx;

namespace UI.CharacterCreator
{
	public class LifepathListView : ScrollViewController<LifepathViewsHolder>
	{
		private ScrollViewItemList<LifepathAsset> items;
		private Subject<string> lifepathObservable = new Subject<string>();

		public IObservable<string> LifepathObservable => lifepathObservable;
		
		public void SetItems(IList<LifepathAsset> lifepaths)
		{
			//Init();
			this.items.SetItems(lifepaths);
		}
		
		/// <inheritdoc />
		protected override void Awake()
		{
			items = new ScrollViewItemList<LifepathAsset>(this);
			base.Awake();
		}

		/// <inheritdoc />
		protected override LifepathViewsHolder CreateModel(int itemIndex)
		{
			var vh = new LifepathViewsHolder(itemIndex);
			
			//vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex, false);

			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(LifepathViewsHolder newOrRecycled)
		{
			LifepathAsset model = items[newOrRecycled.ItemIndex];

			newOrRecycled.Callback = OnLifepathSelected;
			newOrRecycled.SetData(model);
            
			//ScheduleComputeVisibilityTwinPass();
		}

		private void OnLifepathSelected(string id)
		{
			this.lifepathObservable.OnNext(id);
		}
	}
}