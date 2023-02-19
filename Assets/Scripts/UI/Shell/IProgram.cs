using OS.Devices;
using UI.Windowing;

namespace UI.Shell
{
	public interface IProgram<TProgramUserInterface>
	{
		void InstantiateIntoWindow(ISystemProcess process, IWindowWithClient<TProgramUserInterface> window);
	}
}