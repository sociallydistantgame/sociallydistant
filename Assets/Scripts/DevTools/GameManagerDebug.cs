#nullable enable

using Core;
using GamePlatform;
using UnityEngine;

namespace DevTools
{
	public class GameManagerDebug : IDevMenu
	{
		private GameManagerHolder gameManager;
		
		public string Name => "Game Manager Debug";

		public GameManagerDebug(GameManagerHolder gameManager)
		{
			this.gameManager = gameManager;
		}
		
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (gameManager.Value == null)
			{
				GUILayout.Label("Sorry, GameManager isn't available!");
				return;
			}

			if (gameManager.Value.CurrentGameMode == GameMode.ThemeCreator)
			{
				if (GUILayout.Button("Exit Theme Editor"))
					gameManager.Value.GoToLoginScreen();
			}

			if (gameManager.Value.CurrentGameMode != GameMode.ThemeCreator)
			{
				if (GUILayout.Button("Enter Theme Editor"))
					gameManager.Value.EnterThemeEditor();
			}
			
			if (gameManager.Value.IsGameActive)
			{
				if (GUILayout.Button("Save Current Game"))
					gameManager.Value.SaveCurrentGame();

				if (GUILayout.Button("End Current Game"))
					gameManager.Value.EndCurrentGame();
			}
			else
			{
				GUILayout.Label("Game is not active");
			}
		}
	}
}