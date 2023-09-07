#nullable enable
using OS.Devices;
using Shell.Windowing;
using UI.Windowing;

namespace Architecture
{
	public interface IProgramOpenHandler
	{
		void OnProgramOpen(ISystemProcess process, IWindow window, ITextConsole console);
	}
}