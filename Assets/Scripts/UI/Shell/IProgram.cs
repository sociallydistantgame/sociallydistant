using OS.Devices;
using UI.Windowing;

namespace UI.Shell
{
	public interface IProgram<TProgramUserInterface>
	{
		string BinaryName { get; }
		string WindowTitle { get; }
		
		void InstantiateIntoWindow(ISystemProcess process, IWindowWithClient<TProgramUserInterface> window, ITextConsole console);
	}
}