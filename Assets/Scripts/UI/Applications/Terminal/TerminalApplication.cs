#nullable enable

using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Architecture;
using Core.Scripting;
using OS.Devices;
using Shell.Windowing;
using UI.Shell;
using UI.Terminal.SimpleTerminal;
using UI.UiHelpers;
using UI.Windowing;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace UI.Applications.Terminal
{
	public class TerminalApplication :
		MonoBehaviour,
		IProgramOpenHandler,
		IWindowCloseBlocker
	{
		private OperatingSystemExecutionContext context = null!;
		private ISystemProcess process = null!;
		private IContentPanel window = null!;
		private ISystemProcess? shellProcess;
		private SimpleTerminalRenderer st = null!;
		private ITextConsole? textConsole;
		private InteractiveShell? shell;
		private DialogHelper dialogHelper = null!;
		private bool isWaitingForInput;
		private bool hasStartedShell;
		private Task? shellTask;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TerminalApplication));
			this.MustGetComponentInChildren(out st);
			this.MustGetComponent(out dialogHelper);
		}

		private async Task StartShell()
		{
			// Start the terminal session, this allows us to receive input
			// and send characters to it.
			this.textConsole = st.StartSession();
			
			// Move to the user's home directory.
			this.process.WorkingDirectory = this.process.User.Home;
			
			// Fork a child process for the shell or specified command-line
			// application to use.
			this.shellProcess = this.process.Fork();
			
			// Create a shell execution context for the in-game OS.
			this.context = new OperatingSystemExecutionContext(shellProcess);
			
			// TODO: Command-line arguments to specify the shell
			// Create a shell.
			this.shell = new InteractiveShell(context);
			shell.HandleExceptionsGracefully = true;
			shell.Setup(shellProcess, textConsole);

			while (shellProcess.IsAlive)
			{
				try
				{
					await shell.Run();
				}
				catch (ScriptEndException endException)
				{
					shellProcess.Kill(endException.ExitCode);
				}
			}
		}

		private void Update()
		{
			if (shellTask == null)
			{
				shellTask = this.StartShell();
				hasStartedShell = true;
			}

			if (shellTask.IsCompleted)
				this.process.Kill();
		}

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args)
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
						this.window.Window,
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