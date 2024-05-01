#nullable enable
using Core;
using GameplaySystems.Networld;
using OS.Devices;
using UI.Backdrop;
using UI.Login;
using UI.PlayerUI;
using UI.Popovers;
using UI.Shell;
using UI.Windowing;
using UnityEngine;

namespace Player
{
	public struct PlayerInstance
	{
		public IInitProcess OsInitProcess;
		public PlayerComputer Computer;

		public UiManager UiManager;
		
		public GameObject UiRoot;
		public PlayerFileOverrider FileOverrider;

		public LocalAreaNetwork PlayerLan;

		public ISkillTree SkillTree;
	}
}