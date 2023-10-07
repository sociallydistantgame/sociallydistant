#nullable enable

using Core;
using UnityEngine;

namespace DevTools.Social
{
	public class SocialDebug : IDevMenu
	{
		private WorldManagerHolder worldHolder;

		/// <inheritdoc />
		public string Name => "Social + Chat Tools";

		public SocialDebug(WorldManagerHolder world)
		{
			this.worldHolder = world;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			WorldManager? world = worldHolder.Value;

			if (world == null)
			{
				GUILayout.Label("Sorry, WorldManager is unavailable!");
				return;
			}
			
			GUILayout.Label("General tools");

			if (GUILayout.Button("View/manage user profiles"))
				devMenu.PushMenu(new ProfileDebug(world));

			GUILayout.Label("Chat");
			
			if (GUILayout.Button("Manage guilds"))
				devMenu.PushMenu(new GuildDebug(world));
		}
	}
}