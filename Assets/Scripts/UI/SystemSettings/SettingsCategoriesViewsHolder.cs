#nullable enable
using AcidicGui.Components;
using UI.Shell.InfoPanel;
using UnityExtensions;

namespace UI.SystemSettings
{
	public class SettingsCategoriesViewsHolder : AutoSizedItemsViewsHolder
	{
		private ListItemWithHeader view = null!;
		private SettingsCategoryModel? model;
		private SystemSettingsController systemSettings = null!;
		
		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out view);

			view.Activator.onClick.AddListener(OnListItemClick);
			
			view.MustGetComponentInParent(out systemSettings);
			
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
			
			systemSettings.ShowCategory(model);
		}

		/// <inheritdoc />
		public SettingsCategoriesViewsHolder(int itemIndex) : base(itemIndex)
		{ }
	}
}