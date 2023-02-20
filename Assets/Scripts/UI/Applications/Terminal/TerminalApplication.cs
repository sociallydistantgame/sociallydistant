#nullable enable

using System;
using Architecture;
using OS.Devices;
using UI.Shell;
using UI.Terminal.SimpleTerminal;
using UI.Windowing;
using UnityEngine;
using Utility;

namespace UI.Applications.Terminal
{
	public class TerminalApplication :
		MonoBehaviour,
		IProgramOpenHandler
	{
		private ISystemProcess process = null!;
		private IWindow window = null!;
		private ISystemProcess shellProcess;
		private SimpleTerminalRenderer st = null!;
		private ITextConsole textConsole;
		private ITerminalProcessController shell;
		private bool isWaitingForInput;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TerminalApplication));
			this.MustGetComponentInChildren(out st);
		}

		private void Start()
		{
			// Start the terminal session, this allows us to receive input
			// and send characters to it.
			this.textConsole = st.StartSession();
			
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
			shell.Update();
		}

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IWindow window)
		{
			this.process = process;
			this.window = window;
		}
	}
}