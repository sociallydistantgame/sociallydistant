#nullable enable
using AcidicGui.Components;
using UI.Shell.InfoPanel;
using UnityExtensions;
using System;

namespace UI.SystemSettings
{
	public class SettingsCategoriesViewsHolder : AutoSizedItemsViewsHolder
	{
		private ListItemWithHeader view = null!;
		private SettingsCategoryModel? model;

		public Action<SettingsCategoryModel>? ClickCallback;
		
		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out view);

			view.Activator.onClick.AddListener(OnListItemClick);

			base.CollectViews();
		}

		public void SetModel(SettingsCategoryModel model)
		{
			this.model = model;
			view.Value = model.Title;
			this.view.IsActive = model.IsActive;
			view.Title = model.MetaTitle;
			view.ShowTitle = model.ShowTitleArea;
		}

		private void OnListItemClick()
		{
			if (model == null)
				return;

			ClickCallback?.Invoke(model);
		}
	}
}