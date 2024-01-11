using OS.Devices;
using Shell.Common;
using Shell.Windowing;

namespace Shell
{
	public interface IProgram
	{
		string BinaryName { get; }
		string WindowTitle { get; }
		CompositeIcon Icon { get; }
		
		void InstantiateIntoWindow(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args);
	}
}