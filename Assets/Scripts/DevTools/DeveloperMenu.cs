#nullable enable

using System.Collections.Generic;
using Architecture;
using Core;
using GamePlatform;
using GameplaySystems.GameManagement;
using GameplaySystems.Networld;
using JetBrains.Annotations;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace DevTools
{
	public class DeveloperMenu : MonoBehaviour
	{
		private readonly List<IDevMenu> menus = new List<IDevMenu>();
		private readonly Stack<IDevMenu> menuStack = new Stack<IDevMenu>();
		private IDevMenu? currentMenu;
		private bool showDevTools;
		private bool wasTildePressed;
		private Vector2 scrollPosition;

		[Header("Dependencies")]
		[SerializeField]
		private WorldManagerHolder world = null!;

		[SerializeField]
		private DeviceCoordinator deviceCoordinator = null!;
		
		[SerializeField]
		private GameManagerHolder gameManager = null!;

		[SerializeField]
		private PlayerInstanceHolder playerInstance = null!;

		[SerializeField]
		private NetworkSimulationHolder networkSimulation = null!;

		[SerializeField]
		private GamePlatformHolder platform = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(DeveloperMenu));
		}

		private void Start()
		{
			menus.Insert(0, new GameManagerDebug(gameManager));
			menus.Insert(1, new NetworldDebug(world.Value, networkSimulation.Value, playerInstance));
			menus.Insert(2, new GodModeMenu(deviceCoordinator, playerInstance));
			menus.Insert(3, new HackablesMenu(world));
		}

		private Rect GetScreenRect()
		{
			return new Rect(0, 0, Screen.width / 4f, Screen.height * 0.75f);
		}

		private void Update()
		{
			bool isTildePressed = Keyboard.current.backquoteKey.IsPressed();

			if (wasTildePressed != isTildePressed)
			{
				wasTildePressed = isTildePressed;
				if (isTildePressed)
					showDevTools = !showDevTools;
			}
		}

		private void OnGUI()
		{
			if (!showDevTools)
				return;

			GUILayout.BeginArea(GetScreenRect());
			GUILayout.Label("Socially Distant Dev Tools");

			scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
			
			if (currentMenu != null)
			{
				GUILayout.Label(currentMenu.Name);
				currentMenu.OnMenuGUI(this);
				if (GUILayout.Button("Go back"))
				{
					PopMenu();
				}
			}
			else
			{
				foreach (IDevMenu menu in menus)
				{
					if (GUILayout.Button(menu.Name))
					{
						PushMenu(menu);
						break;
					}
				}
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		public void PushMenu(IDevMenu menu)
		{
			if (this.currentMenu != null)
				this.menuStack.Push(this.currentMenu);

			this.currentMenu = menu;
		}

		public void PopMenu()
		{
			if (menuStack.Count > 0)
				currentMenu = menuStack.Pop();
			else
				currentMenu = null;
		}
	}
}