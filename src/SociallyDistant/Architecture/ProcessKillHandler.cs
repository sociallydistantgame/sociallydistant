#nullable enable
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.Architecture
{
	public class ProcessKillHandler : IProgramOpenHandler
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