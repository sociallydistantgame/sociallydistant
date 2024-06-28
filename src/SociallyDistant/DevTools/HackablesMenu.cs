#nullable enable
using ImGuiNET;
using SociallyDistant.Core.Core;

namespace SociallyDistant.DevTools
{
	public class HackablesMenu : IDevMenu
	{
		/// <inheritdoc />
		public string Name => "Hackables Debug";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			IWorldManager world = SociallyDistantGame.Instance.WorldManager;
			
			if (ImGui.Button("Create Hackable"))
				devMenu.PushMenu(new CreateHackableMenu(world));
		}
	}
}