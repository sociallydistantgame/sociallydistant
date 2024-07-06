#nullable enable

using System.Net.Mime;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGuiNet;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.UI;
using SociallyDistant.DevTools.Social;
using SociallyDistant.UI;

namespace SociallyDistant.DevTools
{
	public class DeveloperMenu
	{
		private readonly SociallyDistantGame gameManager;
		
		private readonly List<IDevMenu> menus = new List<IDevMenu>();
		private readonly Stack<IDevMenu> menuStack = new Stack<IDevMenu>();
		private IDevMenu? currentMenu;
		private bool showDevTools;
		private bool showGuiInspector;
		private Vector2 scrollPosition;
		private GuiService guiController;

		internal DeveloperMenu()
		{
			this.gameManager = SociallyDistantGame.Instance;
		}

		internal void Initialize()
		{
			gameManager.MustGetComponent(out guiController);
			
			menus.Add(new SettingsDebugMenu());
			menus.Add(new UriRunnerMenu(gameManager));
			menus.Add(new GameManagerDebug(gameManager));
			menus.Add(new HackablesMenu());
			menus.Add(new SocialDebug());
			menus.Add(new EmailMenu(gameManager.SocialService));
			menus.Add(new ConversationScriptDebugger());
		}

		internal void ToggleVisible()
		{
			this.showDevTools = !showDevTools;
		}

		public void ToggleGuiInspector()
		{
			showGuiInspector = !showGuiInspector;
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

		internal void OnGUI()
		{
			if (showGuiInspector && guiController != null)
				GuiInspector.DoImgui(guiController);
			
			if (!showDevTools)
				return;
			
			ImGui.Begin("Socially Distant Operator's Menu");

			if (currentMenu != null)
			{
				if (ImGui.Button("Back"))
				{
					PopMenu();
				}
				else
				{
					ImGui.Text(currentMenu.Name);
					
					currentMenu.OnMenuGUI(this);
				}
			}
			else
			{
				ImGui.Text("Choose Menu");
				
				foreach (IDevMenu menu in menus)
				{
					if (ImGui.Button(menu.Name))
					{
						PushMenu(menu);
						break;
					}
				}
			}
			
			ImGui.End();
		}
	}

	public class DebugHelpers
	{
		public static void SaveAndOpenTexture(Texture2D texture)
		{
			string folder = Path.Combine(SociallyDistantGame.GameDataPath, "temp");
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			string filepath = Path.Combine(folder, Path.GetTempFileName() + ".png");

			using (var stream = File.OpenWrite(filepath))
			{
				int width = texture.Width;
				int height = texture.Height;
				texture.SaveAsPng(stream, width, height);
			}

			System.Diagnostics.Process.Start(filepath);
		}
	}
}