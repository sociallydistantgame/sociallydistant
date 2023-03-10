#nullable enable
using Core;
using Core.WorldData.Data;
using UnityEngine;
using Utility;

namespace DevTools
{
	public sealed class CreateComputerMenu : IDevMenu
	{
		private WorldManager world;
		private WorldComputerData computer;


		/// <inheritdoc />
		public string Name => "Create Computer";

		public CreateComputerMenu(WorldManager world)
		{
			this.world = world;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name:");
			GUILayout.FlexibleSpace();
			computer.HostName = GUILayout.TextField(computer.HostName);
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Create"))
			{
				computer.InstanceId = world.GetNextObjectId();
				computer.MacAddress = (long) Random.Range(0, long.MaxValue >> 16);
				
				world.World.Computers.Add(computer);
				devMenu.PopMenu();
			}
		}
	}
}