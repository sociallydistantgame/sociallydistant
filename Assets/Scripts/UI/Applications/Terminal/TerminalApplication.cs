#nullable enable

using System;
using Architecture;
using OS.Devices;
using UI.Shell;
using UI.Terminal.SimpleTerminal;
using UI.UiHelpers;
using UI.Windowing;
using UnityEngine;
using Utility;

namespace UI.Applications.Terminal
{
	public class TerminalApplication :
		MonoBehaviour,
		IProgramOpenHandler,
		IWindowCloseBlocker
	{
		private ISystemProcess process = null!;
		private IWindow window = null!;
		private ISystemProcess? shellProcess;
		private SimpleTerminalRenderer st = null!;
		private ITextConsole? textConsole;
		private ITerminalProcessController? shell;
		private DialogHelper dialogHelper = null!;
		private bool isWaitingForInput;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TerminalApplication));
			this.MustGetComponentInChildren(out st);
			this.MustGetComponent(out dialogHelper);
		}

		private void Start()
		{
			// Start the terminal session, this allows us to receive input
			// and send characters to it.
			this.textConsole = st.StartSession();
			
			// Move to the user's home directory.
			this.process.WorkingDirectory = this.process.User.Home;
			
			// Fork a child process for the shell or specified command-line
			// application to use.
			this.shellProcess = this.process.Fork();
			this.shellProcess.Killed += HandleShellKilled;
			
			// TODO: Command-line arguments to specify the shell
			// Create a shell.
			this.shell = new InteractiveShell(this);
			shell.Setup(shellProcess, textConsole);
		}

		private void HandleShellKilled(ISystemProcess process)
		{
			process.Killed -= HandleShellKilled;
			this.process.Kill();
		}

		private void Update()
		{
			shell?.Update();
		}

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IWindow window, ITextConsole console)
		{
			this.process = process;
			this.window = window;
		}

		/// <inheritdoc />
		public bool CheckCanClose()
		{
			if (shell == null)
				return true;
			
			if (shell.IsExecutionHalted)
			{
				if (!dialogHelper.AreAnyDialogsOpen)
				{
					dialogHelper.AskQuestion(
						"Force-quit running tasks?",
						"There are tasks currently running inside this Terminal. Are you sure you want to quit the Terminal and force-quit all currently-running tasks? Any unsaved data will be lost.",
						this.window,
						result =>
						{
							if (result)
								process.Kill();
						}
					);
				}
				
				return false;
			}

			return true;
		}
	}
}