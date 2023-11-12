#nullable enable

using System;
using Architecture;
using OS.Devices;
using Player;
using Shell.Windowing;
using TMPro;
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
		IProgramOpener<RectTransform>,
		IDesktop
	{
		[Header("Dependencies")]
		[SerializeField]
		private PlayerInstanceHolder playerHolder = null!;

		[Header("UI")]
		[SerializeField]
		private Button systemSettingsButton = null!;
		
		[SerializeField]
		private RectTransform workspaceArea = null!;

		[SerializeField]
		private TextMeshProUGUI shellUserDisplayTText = null!;
		
		private IWorkspaceDefinition currentWorkspace = null!;
		private ISystemProcess loginProcess = null!;
		private IUser loginUser = null!;
		private UiManager uiManager = null!;

		public IWorkspaceDefinition CurrentWorkspace => currentWorkspace;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(Desktop));
			this.MustGetComponentInParent(out uiManager);
		}

		private void Start()
		{
			this.loginUser = this.playerHolder.Value.Computer.PlayerUser;
			this.loginProcess = this.playerHolder.Value.OsInitProcess.CreateLoginProcess(this.loginUser);
			this.currentWorkspace = playerHolder.Value.UiManager.WindowManager.DefineWorkspace(this.workspaceArea);
			this.systemSettingsButton.onClick.AddListener(uiManager.OpenSettings);

			this.UpdateUserDisplay();
		}

		/// <inheritdoc />
		public ISystemProcess OpenProgram(IProgram<RectTransform> program, string[] arguments, ISystemProcess? parentProcess, ITextConsole? console)
		{
			// Create a process for the window, if we weren't supplied with one.
			ISystemProcess windowProcess = parentProcess ?? this.loginProcess.Fork();
			
			// Create a new window for the program, on the current workspace.
			IWindowWithClient<RectTransform>? win = CurrentWorkspace.CreateWindow("Window") as IWindowWithClient<RectTransform>;
			if (win == null)
				throw new InvalidOperationException("Cannot launch a program window because the workspace didn't create a valid window.");

			// Spawn the program
			program.InstantiateIntoWindow(windowProcess, win, console, arguments);
			return windowProcess;
		}

		private void UpdateUserDisplay()
		{
			string username = this.loginUser.UserName;
			string hostname = this.loginUser.Computer.Name;
			
			this.shellUserDisplayTText.SetText($"{username}@{hostname}");
		}
	}
}