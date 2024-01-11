#nullable enable
using OS.Devices;
using Shell.Windowing;
using UI.Windowing;

namespace Architecture
{
	public interface IProgramOpenHandler
	{
		void OnProgramOpen(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args);
	}
}