using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools.Social
{
	public class CreateGuildMenu : IDevMenu
	{
		private readonly WorldManager world;
		private WorldGuildData guildData;

		/// <inheritdoc />
		public string Name => "Create New Guild";

		public CreateGuildMenu(WorldManager world)
		{
			this.world = world;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			ImGui.Text("Enter guild name");

			//guildData.Name = GUILayout.TextField(guildData.Name);
            
			if (ImGui.Button("Create"))
			{
				guildData.InstanceId = world.GetNextObjectId();
				
				world.World.Guilds.Add(guildData);

				devMenu.PopMenu();
				devMenu.PushMenu(new GuildInfoMenu(world, guildData.InstanceId));
			}
		}
	}
}