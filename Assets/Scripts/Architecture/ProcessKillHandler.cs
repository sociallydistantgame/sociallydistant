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
		private IContentPanel window = null!;
		private ISystemProcess process = null!;
		private bool killedFirst;

		/// <inheritdoc />
		public void OnProgramOpen(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args)
		{
			this.process = process;
			this.window = window;
			
			this.process.Killed += HandleProcessKilled;
		}

		private void HandleProcessKilled(ISystemProcess obj)
		{
			obj.Killed -= HandleProcessKilled;
			
			if (!killedFirst)
			{
				killedFirst = true;
				window.ForceClose();
			}
		}

		private void HandleWindowClosed(IWindow obj)
		{
			if (!killedFirst)
			{
				killedFirst = true;
				process.Kill();
			}

			window.Window.WindowClosed -= HandleWindowClosed;
		}
	}
}