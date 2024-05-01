#nullable enable
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

namespace UI.SystemSettings
{
	public class SettingsCategoriesList : OSA<BaseParamsWithPrefab, SettingsCategoriesViewsHolder>
	{
		private SimpleDataHelper<SettingsCategoryModel> models;

		/// <inheritdoc />
		protected override void Start()
		{
			models = new SimpleDataHelper<SettingsCategoryModel>(this);
			base.Start();
		}

		/// <inheritdoc />
		protected override SettingsCategoriesViewsHolder CreateViewsHolder(int itemIndex)
		{
			var vh = new SettingsCategoriesViewsHolder();

			vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateViewsHolder(SettingsCategoriesViewsHolder newOrRecycled)
		{
			SettingsCategoryModel? model = models[newOrRecycled.ItemIndex];

			newOrRecycled.SetModel(model);
			
			ScheduleComputeVisibilityTwinPass();
		}

		public void SetItems(IList<SettingsCategoryModel> modelList)
		{
			this.models.ResetItems(modelList);
		}
	}
}