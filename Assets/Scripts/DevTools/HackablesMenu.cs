#nullable enable
using Core;
using GamePlatform;
using UnityEngine;

namespace DevTools
{
	public class HackablesMenu : IDevMenu
	{
		/// <inheritdoc />
		public string Name => "Hackables Debug";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			IWorldManager world = GameManager.Instance.WorldManager;
			
			if (GUILayout.Button("Create Hackable"))
				devMenu.PushMenu(new CreateHackableMenu(world));
		}
	}
}