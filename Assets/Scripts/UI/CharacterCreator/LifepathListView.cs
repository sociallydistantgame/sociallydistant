using System;
using System.Collections.Generic;
using Architecture;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using Cysharp.Threading.Tasks;
using UniRx;

namespace UI.CharacterCreator
{
	public class LifepathListView : OSA<BaseParamsWithPrefab, LifepathViewsHolder>
	{
		private SimpleDataHelper<LifepathAsset> items;
		private Subject<string> lifepathObservable = new Subject<string>();

		public IObservable<string> LifepathObservable => lifepathObservable;
		
		public void SetItems(IList<LifepathAsset> lifepaths)
		{
			//Init();
			this.items.ResetItems(lifepaths);
		}
		
		/// <inheritdoc />
		protected override void Awake()
		{
			items = new SimpleDataHelper<LifepathAsset>(this);
			base.Awake();
		}

		/// <inheritdoc />
		protected override LifepathViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new LifepathViewsHolder();
			
			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex, false);

			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(LifepathViewsHolder newOrRecycled)
		{
			LifepathAsset model = items[newOrRecycled.ItemIndex];

			newOrRecycled.Callback = OnLifepathSelected;
			newOrRecycled.SetData(model);
            
			ScheduleComputeVisibilityTwinPass();
		}

		private void OnLifepathSelected(string id)
		{
			this.lifepathObservable.OnNext(id);
		}
	}
}