#nullable enable

using System;
using Architecture;
using GamePlatform;
using Modules;
using OS.Devices;
using Player;
using Shell;
using Shell.Windowing;
using TMPro;
using UI.Controllers;
using UI.PlayerUI;
using UI.Windowing;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using Utility;
using System.Threading.Tasks;

namespace UI.Shell
{
	public class Desktop :
		MonoBehaviour,
		IProgramOpener,
		IDesktop
	{
		[SerializeField]
		private PlayerInstanceHolder playerHolder = null!;

		[Header("UI")]
		[SerializeField]
		private RectTransform workspaceArea = null!;

		[SerializeField]
		private TabbedToolManager toolManager = null!;
		
		[SerializeField]
		private StatusBarController statusBarController = null!;
		
		private IWorkspaceDefinition currentWorkspace = null!;
		private ISystemProcess loginProcess = null!;
		private IUser loginUser = null!;
		private UiManager uiManager = null!;
		private CanvasGroup canvasGroup = null!;
		private LTDescr? fadeIn;
		private LTDescr? fadeOut;
		private GameManager gameManager = null!;
		
		internal ISystemProcess LoginProcess => loginProcess;
		
		public IWorkspaceDefinition CurrentWorkspace => currentWorkspace;
		
		private void Awake()
		{
			gameManager = GameManager.Instance;
			this.AssertAllFieldsAreSerialized(typeof(Desktop));
			this.MustGetComponentInParent(out uiManager);
			this.MustGetComponent(out canvasGroup);

			canvasGroup.alpha = 0;
		}

		private async void OnEnable()
		{
			this.loginUser = this.playerHolder.Value.Computer.PlayerUser;
			this.loginProcess = await this.playerHolder.Value.OsInitProcess.CreateLoginProcess(this.loginUser);
			
			gameManager.UriManager.RegisterSchema("web", new BrowserSchemeHandler(this));
			gameManager.UriManager.RegisterSchema("shell", new ShellUriSchemeHandler(this));
		
			this.UpdateUserDisplay();
			await this.toolManager.StartFirstTool();
			Show();
		}

		private void OnDisable()
		{
			gameManager.UriManager.UnregisterSchema("web");
			gameManager.UriManager.UnregisterSchema("shell");

			Hide();
		}

		private void Start()
		{
			this.currentWorkspace = playerHolder.Value.UiManager.WindowManager.DefineWorkspace(this.workspaceArea);
		}

		/// <inheritdoc />
		public async Task<ISystemProcess> OpenProgram(IProgram program, string[] arguments, ISystemProcess? parentProcess, ITextConsole? console)
		{
			// Create a process for the window, if we weren't supplied with one.
			ISystemProcess windowProcess = parentProcess ?? await this.loginProcess.Fork();
			
			// Create a new window for the program, on the current workspace.
			IFloatingGui? win = CurrentWorkspace.CreateFloatingGui("Window");
			if (win == null)
				throw new InvalidOperationException("Cannot launch a program window because the workspace didn't create a valid window.");
			
			// Spawn the program
			program.InstantiateIntoWindow(windowProcess, win.ActiveContent, console, arguments);
			return windowProcess;
		}

		public async Task OpenWebBrowser(Uri uri)
		{
			await this.toolManager.OpenWebBrowser(uri);
		}

		private void UpdateUserDisplay()
		{
			string username = this.loginUser.UserName;
			string hostname = this.loginUser.Computer.Name;
			
			statusBarController.UserInfo = $"{username}@{hostname}";
		}

		private void Show()
		{
			if (fadeOut != null)
			{
				LeanTween.cancel(fadeOut.id);
				fadeOut = null;
			}
			
			if (fadeIn != null)
				return;
			
			fadeIn = LeanTween.alphaCanvas(canvasGroup, 1, 0.5f).setOnComplete(AfterHide);
		}

		private void Hide()
		{
			if (fadeIn != null)
			{
				LeanTween.cancel(fadeIn.id);
				fadeIn = null;
			}

			if (fadeOut != null)
				return;

			fadeOut = LeanTween.alphaCanvas(canvasGroup, 0, 0.5f).setOnComplete(AfterHide);
		}

		private void AfterShow()
		{
			fadeIn = null;
		}

		private void AfterHide()
		{
			fadeOut = null;
		}

		private sealed class ShellUriSchemeHandler : IUriSchemeHandler
		{
			private readonly Desktop shell;

			public ShellUriSchemeHandler(Desktop shell)
			{
				this.shell = shell;
			}
			
			/// <inheritdoc />
			public void HandleUri(Uri uri)
			{
				switch (uri.Host)
				{
					case "tool":
					{
						string toolId = uri.AbsolutePath.Substring(1);
						shell.toolManager.SwitchTools(toolId);
						break;
					}
				}
			}
		}
	}
}