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
		
		private SettingsCategoriesList categoriesList = null!;

		
		private SystemSettingsViewController viewController = null!;
		
		private readonly Dictionary<SettingsCategoryModel, SettingsCategoryView> views = new Dictionary<SettingsCategoryModel, SettingsCategoryView>();
		
		private GameManager gameManager = null!;
		private IWindow window = null!;
		private UiManager uiManager = null!;
		private DialogHelper dialogHelper = null!;
		private ISettingsManager settingsService = null!;
		
		private async UniTaskVoid Start()
		{
			if (allCategories.Count == 0)
			{
				dialogHelper.ShowMessage(MessageBoxType.Error, "System Settings Service - Error", "System Settings could not find any system settings. The window will now close. Please check the Unity logs for any unhandled exceptions.", this.window, this.window.Close);
				return;
			}
		}
	}
}