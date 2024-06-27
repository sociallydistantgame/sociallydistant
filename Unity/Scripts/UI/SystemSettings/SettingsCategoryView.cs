#nullable enable
using System;
using AcidicGui.Mvc;

namespace UI.SystemSettings
{
	public class SettingsCategoryView : IViewWithData<SettingsCategoryModel>
	{
		private readonly SystemSettingsViewController controller;
		private SettingsCategoryModel? model;

		
		
		public SettingsCategoryView(SystemSettingsViewController controller)
		{
			this.controller = controller;
		}
		
		/// <inheritdoc />
		public void SetData(SettingsCategoryModel data)
		{
			this.model = data;

			this.controller.ShowHeader = model.ShowTitleArea;
			this.controller.CategoryTitle = $"{this.model.Category?.SectionName ?? "Settings"} > {model.Title}";

			if (this.model.Category == null)
			{
				this.controller.HideWidgets();
			}
			else
			{
				this.controller.ShowWidgets(model.Category);
			}
		}

		/// <inheritdoc />
		public bool IsVisible => controller.IsVisible;

		/// <inheritdoc />
		public void Show(Action? callback = null)
		{
			controller.Show(callback);
		}

		/// <inheritdoc />
		public void Hide(Action? callback = null)
		{
			controller.Hide(callback);
		}
	}
}