#nullable enable
using OS.Devices;
using UI.Windowing;

namespace Architecture
{
	public interface IProgramOpenHandler
	{
		void OnProgramOpen(ISystemProcess process, IWindow window);
	}
}