#nullable enable
using Architecture;
using OS.Devices;
using Player;
using UnityEditor;
using UnityEngine;

namespace DevTools
{
	public class GodModeMenu : IDevMenu
	{
		private DeviceCoordinator coordinator;
		private PlayerInstanceHolder playerInstance = null!;
		
		/// <inheritdoc />
		public string Name => "God Mode (Run Any GUI Application on Any Device As Root)";

		public GodModeMenu(DeviceCoordinator coordinator, PlayerInstanceHolder playerInstance)
		{
			this.coordinator = coordinator;
			this.playerInstance = playerInstance;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			GUILayout.Label("Select Computer");

			foreach (IInitProcess initProcess in coordinator.GetAllRootTasks())
			{
				IComputer computer = initProcess.User.Computer;
				
				if (GUILayout.Button(computer.Name))
					devMenu.PushMenu(new GodModeAppChooser(initProcess, playerInstance));
			}
		}
	}
}