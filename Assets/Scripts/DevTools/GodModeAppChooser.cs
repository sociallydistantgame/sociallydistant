#nullable enable
using System;
using Architecture;
using OS.Devices;
using Player;
using UnityEngine;

namespace DevTools
{
	public class GodModeAppChooser : IDevMenu
	{
		private ISystemProcess process;
		private PlayerInstanceHolder playerInstance;
		private UguiProgram[] programs;

		/// <inheritdoc />
		public string Name => "Choose app";

		public GodModeAppChooser(ISystemProcess process, PlayerInstanceHolder playerInstance)
		{
			this.process = process;
			this.playerInstance = playerInstance;
			this.programs = Resources.LoadAll<UguiProgram>("Applications");
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			GUILayout.Label("Select Application");

			foreach (UguiProgram program in programs)
			{
				if (!GUILayout.Button(program.WindowTitle))
					continue;
				
				if (this.playerInstance.Value.UiManager.Desktop != null)
					playerInstance.Value.UiManager.Desktop.OpenProgram(program, Array.Empty<string>(), process, null);
			}
		}
	}
}