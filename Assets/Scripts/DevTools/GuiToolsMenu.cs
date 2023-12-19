#nullable enable
using System;
using Player;
using UI.PlayerUI;
using UnityEngine;

namespace DevTools
{
	public class GuiToolsMenu : IDevMenu
	{
		private readonly PlayerInstanceHolder playerInstance;
		
		/// <inheritdoc />
		public string Name => "GUI tools";

		public GuiToolsMenu(PlayerInstanceHolder holder)
		{
			this.playerInstance = holder;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (playerInstance.Value.UiManager == null)
			{
				GUILayout.Label("Sorry, UiManager isn't available!");
				return;
			}

			GuiTool("Set Gui Skin", devMenu, SetGuiSkin);
		}

		private void GuiTool(string name, DeveloperMenu devMenu, Action<DeveloperMenu, UiManager> action)
		{
			if (!GUILayout.Button(name))
				return;
			
			action?.Invoke(devMenu, playerInstance.Value.UiManager);
		}

		private void SetGuiSkin(DeveloperMenu devMenu, UiManager uiManager)
		{
			devMenu.PushMenu(new DevSkinLoader(uiManager));
		}
	}
}