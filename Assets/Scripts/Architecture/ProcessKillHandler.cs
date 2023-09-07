#nullable enable
using OS.Devices;
using Shell.Windowing;
using UI.Windowing;
using UnityEngine;

namespace Architecture
{
	public class ProcessKillHandler :
		MonoBehaviour,
		IProgramOpenHandler
	{
		private IWindow window = null!;
		private ISystemProcess process = null!;
		private bool killedFirst;

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IWindow window, ITextConsole console)
		{
			this.process = process;
			this.window = window;
			
			this.window.WindowClosed += HandleWindowClosed;
			this.process.Killed += HandleProcessKilled;
		}

		private void HandleProcessKilled(ISystemProcess obj)
		{
			if (!killedFirst)
			{
				killedFirst = true;
				window.ForceClose();
			}

			process.Killed -= HandleProcessKilled;
		}

		private void HandleWindowClosed(IWindow obj)
		{
			if (!killedFirst)
			{
				killedFirst = true;
				process.Kill();
			}

			window.WindowClosed -= HandleWindowClosed;
		}
	}
}