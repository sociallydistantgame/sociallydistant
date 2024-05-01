#nullable enable

using Core;
using GamePlatform;
using UnityEngine;

namespace DevTools
{
	public class GameManagerDebug : IDevMenu
	{
		private GameManager gameManager;
		
		public string Name => "Game Manager Debug";

		public GameManagerDebug(GameManager gameManager)
		{
			this.gameManager = gameManager;
		}
		
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (gameManager.IsGameActive)
			{
				if (GUILayout.Button("Save Current Game"))
					gameManager.SaveCurrentGame(false);

				if (GUILayout.Button("End Current Game"))
					gameManager.EndCurrentGame(true);
			}
			else
			{
				GUILayout.Label("Game is not active");
			}
		}
	}
}