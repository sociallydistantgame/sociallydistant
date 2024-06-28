#nullable enable

using System.Collections.Generic;
using System.IO;
using Architecture;
using Core;
using GamePlatform;
using GameplaySystems.Networld;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtensions;
using DevTools.Social;
using GameplaySystems.Chat;
using GameplaySystems.Missions;
using GameplaySystems.Social;

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
		
		private SocialServiceHolder socialServiceHolder = null!;
		
		
		private DeviceCoordinator deviceCoordinator = null!;
		
		
		private PlayerInstanceHolder playerInstance = null!;

		
		private NetworkSimulationHolder networkSimulation = null!;

		
		private GamePlatformHolder platform = null!;
		
		private GameManager gameManager = null!;
		
		private void Awake()
		{
			gameManager = GameManager.Instance;
			
			this.AssertAllFieldsAreSerialized(typeof(DeveloperMenu));
		}

		private Texture2D overlay;

		private void Start()
		{
			menus.Insert(0, new UriRunnerMenu(gameManager));
			menus.Insert(1, new GameManagerDebug(gameManager));
			menus.Insert(2, new NetworldDebug(networkSimulation.Value, playerInstance));
			menus.Insert(3, new GodModeMenu(deviceCoordinator, playerInstance));
			menus.Insert(4, new HackablesMenu());
			menus.Insert(5, new SocialDebug());
			menus.Insert(6, new GuiToolsMenu(playerInstance));
			menus.Insert(7, new EmailMenu(this.socialServiceHolder));
			menus.Insert(8, new MissionDebug(this.gameManager));
			menus.Insert(9, new ConversationScriptDebugger());
			

			overlay = new Texture2D(1, 1);
			overlay.SetPixel(0, 0, Color.black);
		}

		private void OnDestroy()
		{
			Destroy(overlay);
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

			Rect screenRect = GetScreenRect();

			GUI.color = Color.black;
			GUI.DrawTexture(screenRect, overlay);
			GUI.color = Color.white;
			
			GUILayout.BeginArea(screenRect);
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

	public class DebugHelpers
	{
		public static void SaveAndOpenTexture(Texture2D texture)
		{
			string folder = Path.Combine(Application.persistentDataPath, "temp");
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			string filepath = Path.Combine(folder, Path.GetTempFileName() + ".png");

			byte[] data = texture.EncodeToPNG();

			using (FileStream stream = File.Open(filepath, FileMode.OpenOrCreate))
			{
				stream.SetLength(data.Length);
				stream.Write(data, 0, data.Length);
			}

			System.Diagnostics.Process.Start(filepath);
		}
	}
}