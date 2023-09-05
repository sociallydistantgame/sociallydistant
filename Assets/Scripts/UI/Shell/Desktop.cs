#nullable enable

using System;
using Architecture;
using OS.Devices;
using Player;
using UI.Windowing;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace UI.Shell
{
	public class Desktop :
		MonoBehaviour,
		IProgramOpener<RectTransform>,
		IDesktop
	{
		private IWorkspaceDefinition currentWorkspace = null!;
		private ISystemProcess loginProcess = null!;
		private IUser loginUser = null!;

		[Header("Dependencies")]
		[SerializeField]
		private PlayerInstanceHolder playerHolder = null!;

		[Header("UI")]
		[SerializeField]
		private RectTransform workspaceArea = null!;

		public IWorkspaceDefinition CurrentWorkspace => currentWorkspace;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(Desktop));
		}

		private void Start()
		{
			this.loginUser = this.playerHolder.Value.Computer.PlayerUser;
			this.loginProcess = this.playerHolder.Value.OsInitProcess.CreateLoginProcess(this.loginUser);
			this.currentWorkspace = playerHolder.Value.UiManager.WindowManager.DefineWorkspace(this.workspaceArea); 
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
			program.InstantiateIntoWindow(windowProcess, win, console);
			return windowProcess;
		}
	}
}