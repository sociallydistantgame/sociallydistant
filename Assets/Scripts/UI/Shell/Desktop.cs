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

namespace UI.Shell
{
	public class Desktop :
		MonoBehaviour,
		IProgramOpener,
		IDesktop
	{
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
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

		internal ISystemProcess LoginProcess => loginProcess;
		
		public IWorkspaceDefinition CurrentWorkspace => currentWorkspace;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(Desktop));
			this.MustGetComponentInParent(out uiManager);
		}

		private void OnEnable()
		{
			if (gameManager.Value != null)
			{
				gameManager.Value.UriManager.RegisterSchema("web", new BrowserSchemeHandler(this));
				gameManager.Value.UriManager.RegisterSchema("shell", new ShellUriSchemeHandler(this));
			}
		}

		private void OnDisable()
		{
			if (gameManager.Value != null)
			{
				gameManager.Value.UriManager.UnregisterSchema("web");
				gameManager.Value.UriManager.UnregisterSchema("shell");
			}
		}

		private void Start()
		{
			this.loginUser = this.playerHolder.Value.Computer.PlayerUser;
			this.loginProcess = this.playerHolder.Value.OsInitProcess.CreateLoginProcess(this.loginUser);
			this.currentWorkspace = playerHolder.Value.UiManager.WindowManager.DefineWorkspace(this.workspaceArea);

			this.UpdateUserDisplay();
		}

		/// <inheritdoc />
		public ISystemProcess OpenProgram(IProgram program, string[] arguments, ISystemProcess? parentProcess, ITextConsole? console)
		{
			// Create a process for the window, if we weren't supplied with one.
			ISystemProcess windowProcess = parentProcess ?? this.loginProcess.Fork();
			
			// Create a new window for the program, on the current workspace.
			IFloatingGui? win = CurrentWorkspace.CreateFloatingGui("Window");
			if (win == null)
				throw new InvalidOperationException("Cannot launch a program window because the workspace didn't create a valid window.");
			
			// Spawn the program
			program.InstantiateIntoWindow(windowProcess, win.ActiveContent, console, arguments);
			return windowProcess;
		}

		public void OpenWebBrowser(Uri uri)
		{
			this.toolManager.OpenWebBrowser(uri);
		}

		private void UpdateUserDisplay()
		{
			string username = this.loginUser.UserName;
			string hostname = this.loginUser.Computer.Name;
			
			statusBarController.UserInfo = $"{username}@{hostname}";
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