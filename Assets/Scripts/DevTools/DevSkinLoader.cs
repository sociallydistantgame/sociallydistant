#nullable enable
using System;
using AcidicGui.Theming;
using GamePlatform;
using UI.PlayerUI;
using UI.Theming;
using UnityEngine;

namespace DevTools
{
	public class DevSkinLoader : IDevMenu
	{
		private readonly UiManager uiManager = null;
		private readonly GameManager? gameManager = null;
		private bool isLoadingTheme = false;

		/// <inheritdoc />
		public string Name => "Set GUI Skin";

		public DevSkinLoader(UiManager uiManager)
		{
			this.uiManager = uiManager;
			gameManager = UnityEngine.Object.FindObjectOfType<GameManager>();
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (isLoadingTheme)
			{
				GUILayout.Label("Loading theme...");
				return;
			}
			
			if (gameManager == null)
			{
				GUILayout.Label("Failed to locate GameManager!");
				return;
			}
			
			foreach (IThemeAsset theme in gameManager.ContentManager.GetContentOfType<IThemeAsset>())
			{
				if (!GUILayout.Button(theme.Name))
					continue;

				LoadThemeAsync(theme);
				break;
			}
		}

		private async void LoadThemeAsync(IThemeAsset asset)
		{
			try
			{
				isLoadingTheme = true;
				
				OperatingSystemTheme theme = await asset.LoadAsync();
				
				uiManager.ChangeTheme(theme);
			}
			finally
			{
				isLoadingTheme = false;
			}
		}
	}
}