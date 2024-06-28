#nullable enable

using ImGuiNET;
using SociallyDistant.Core;

namespace SociallyDistant.DevTools.Social
{
	public class SocialDebug : IDevMenu
	{
		/// <inheritdoc />
		public string Name => "Social + Chat Tools";
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			var world = WorldManager.Instance; 

			if (world == null)
			{
				ImGui.Text("Sorry, WorldManager is unavailable!");
				return;
			}
			
			ImGui.Text("General tools");

			if (ImGui.Button("View/manage user profiles"))
				devMenu.PushMenu(new ProfileDebug(world));

			ImGui.Text("Chat");
			
			if (ImGui.Button("Manage guilds"))
				devMenu.PushMenu(new GuildDebug(world));
		}
	}
}