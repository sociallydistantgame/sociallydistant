#nullable enable
using GameplaySystems.GameManagement;
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