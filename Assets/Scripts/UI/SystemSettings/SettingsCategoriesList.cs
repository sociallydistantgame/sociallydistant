#nullable enable
using System.Collections.Generic;
using UI.ScrollViews;

namespace UI.SystemSettings
{
	public class SettingsCategoriesList : ScrollViewController<SettingsCategoriesViewsHolder>
	{
		private ScrollViewItemList<SettingsCategoryModel> models;

		/// <inheritdoc />
		protected override void Awake()
		{
			models = new ScrollViewItemList<SettingsCategoryModel>(this);
			base.Awake();
		}

		/// <inheritdoc />
		protected override SettingsCategoriesViewsHolder CreateModel(int itemIndex)
		{
			var vh = new SettingsCategoriesViewsHolder(itemIndex);

			//vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(SettingsCategoriesViewsHolder newOrRecycled)
		{
			SettingsCategoryModel? model = models[newOrRecycled.ItemIndex];

			newOrRecycled.SetModel(model);
		}

		public void SetItems(IList<SettingsCategoryModel> modelList)
		{
			this.models.SetItems(modelList);
		}
	}
}