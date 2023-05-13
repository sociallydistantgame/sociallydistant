#nullable enable
using Core;
using UnityEngine;

namespace DevTools
{
	public class HackablesMenu : IDevMenu
	{
		private readonly WorldManagerHolder world;

		public HackablesMenu(WorldManagerHolder world)
		{
			this.world = world;
		}

		/// <inheritdoc />
		public string Name => "Hackables Debug";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (GUILayout.Button("Create Hackable"))
				devMenu.PushMenu(new CreateHackableMenu(world.Value));
		}
	}
}