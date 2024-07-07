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
		
		/// <inheritdoc />
		public override void CollectViews()
		{
			root.MustGetComponentInChildren(out view);

			view.Activator.onClick.AddListener(OnListItemClick);

			base.CollectViews();
		}

		private void OnListItemClick()
		{
			if (model == null)
				return;

			ClickCallback?.Invoke(model);
		}
	}
}