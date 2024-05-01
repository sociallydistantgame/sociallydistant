using Core;
using Core.WorldData.Data;
using UnityEngine;

namespace DevTools.Social
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
			GUILayout.Label("Enter guild name");

			guildData.Name = GUILayout.TextField(guildData.Name);
            
			if (GUILayout.Button("Create"))
			{
				guildData.InstanceId = world.GetNextObjectId();
				
				world.World.Guilds.Add(guildData);

				devMenu.PopMenu();
				devMenu.PushMenu(new GuildInfoMenu(world, guildData.InstanceId));
			}
		}
	}
}