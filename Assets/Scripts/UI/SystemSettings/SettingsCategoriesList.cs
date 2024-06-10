#nullable enable
using System.Collections.Generic;
using UI.ScrollViews;
using System;

namespace UI.SystemSettings
{
	public class SettingsCategoriesList : ScrollViewController<SettingsCategoriesViewsHolder>
	{
		private ScrollViewItemList<SettingsCategoryModel> models;

		public event Action<SettingsCategoryModel>? OnItemClicked;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			models = new ScrollViewItemList<SettingsCategoryModel>(this);
			base.Awake();
		}

		/// <inheritdoc />
		protected override SettingsCategoriesViewsHolder CreateModel(int itemIndex)
		{
			var vh = new SettingsCategoriesViewsHolder();
			return vh;
		}

		/// <inheritdoc />
		protected override void UpdateModel(SettingsCategoriesViewsHolder newOrRecycled)
		{
			SettingsCategoryModel? model = models[newOrRecycled.ItemIndex];

			newOrRecycled.ClickCallback = ClickHandler;
			newOrRecycled.SetModel(model);
		}

		public void SetItems(IList<SettingsCategoryModel> modelList)
		{
			this.models.SetItems(modelList);
		}
		
		private void ClickHandler(SettingsCategoryModel model)
		{
			OnItemClicked?.Invoke(model);
		}
	}
}