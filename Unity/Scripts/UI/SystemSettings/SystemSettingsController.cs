#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using AcidicGui.Mvc;
using Core.Config;
using Cysharp.Threading.Tasks;
using GamePlatform;
using Shell;
using Shell.Common;
using Shell.Windowing;
using UI.PlayerUI;
using UI.Shell.Common;
using UI.UiHelpers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityExtensions;

namespace UI.SystemSettings
{
	public class SystemSettingsController : Controller<SettingsCategoryView>
	{
		[SerializeField]
		private SettingsCategoriesList categoriesList = null!;

		[SerializeField]
		private SystemSettingsViewController viewController = null!;
		
		private readonly List<SettingsCategoryModel> allCategories = new List<SettingsCategoryModel>();
		private readonly Dictionary<SettingsCategoryModel, SettingsCategoryView> views = new Dictionary<SettingsCategoryModel, SettingsCategoryView>();
		
		private GameManager gameManager = null!;
		private IWindow window = null!;
		private UiManager uiManager = null!;
		private DialogHelper dialogHelper = null!;
		private ISettingsManager settingsService = null!;
		private SettingsCategoryModel? currentModel;
		private SettingsCategory? currentCategory;

		public SettingsCategoryModel? CurrentModel => currentModel;
		
		private void Awake()
		{
			gameManager = GameManager.Instance;
			this.AssertAllFieldsAreSerialized(typeof(SystemSettingsController));
			this.MustGetComponentInParent(out uiManager);
			this.MustGetComponentInParent(out window);
			this.MustGetComponent(out dialogHelper);

			window.Icon = new CompositeIcon
			{
				textIcon = MaterialIcons.Settings,
				iconColor = Color.white.ToShellColor()
			};
		}

		public void ShowCategory(SettingsCategoryModel model)
		{
			this.categoriesList.OnItemClicked += this.ShowCategory;
			this.currentCategory = model.Category;
			RefreshModels();
			
			
			if (CurrentView == null)
			{
				SettingsCategoryView? view = this.views[model];
				
				view.SetData(model);

				this.NavigateTo(view);
			}
			else
			{
				this.GoBack(() =>
				{
					SettingsCategoryView? view = this.views[model];
				
					view.SetData(model);

					this.NavigateTo(view);
				});
			}
		}

		private void RefreshModels()
		{
			var activeCount = 0;
			foreach (SettingsCategoryModel? model in allCategories)
			{
				model.IsActive = currentCategory == model.Category;
				if (model.IsActive)
					activeCount++;
			}
			
			Assert.IsFalse(activeCount>1);
			this.categoriesList.SetItems(allCategories);
		}
		
		private async UniTaskVoid Start()
		{
			await UniTask.DelayFrame(1);

			this.settingsService = this.gameManager.SettingsManager;

			BuildModelList();

			if (allCategories.Count == 0)
			{
				dialogHelper.ShowMessage(MessageBoxType.Error, "System Settings Service - Error", "System Settings could not find any system settings. The window will now close. Please check the Unity logs for any unhandled exceptions.", this.window, this.window.Close);
				return;
			}
			
			ShowCategory(allCategories.First());
		}

		private void BuildModelList()
		{
			this.allCategories.Clear();
			this.views.Clear();
			
			foreach (string sectionTitle in settingsService.SectionTitles)
			{
				var models = new List<SettingsCategoryModel>();

				foreach (SettingsCategory category in this.settingsService.GetCategoriesInSection(sectionTitle))
				{
					var model = new SettingsCategoryModel
					{
						Title = category.Name,
						MetaTitle = sectionTitle,
						Category = category,
						ShowTitleArea = models.Count == 0,
						IsActive = this.currentCategory == category
						
					};
					
					models.Add(model);
					
					views.Add(model, new SettingsCategoryView(this.viewController));
				}
				
				allCategories.AddRange(models);
			}

			this.categoriesList.SetItems(allCategories);
		}
	}
}