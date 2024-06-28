#nullable enable

using ImGuiNET;

namespace SociallyDistant.DevTools
{
	public class GameManagerDebug : IDevMenu
	{
		private SociallyDistantGame gameManager;
		
		public string Name => "Game Manager Debug";

		internal GameManagerDebug(SociallyDistantGame gameManager)
		{
			this.gameManager = gameManager;
		}
		
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (gameManager.IsGameActive)
			{
				if (ImGui.Button("Save Current Game"))
					gameManager.SaveCurrentGame(false);

				if (ImGui.Button("End Current Game"))
					gameManager.EndCurrentGame(true);
			}
			else
			{
				ImGui.Text("Game is not active");
			}
		}
	}
}