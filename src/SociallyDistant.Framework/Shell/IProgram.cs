using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.Core.Shell
{
	public interface IProgram
	{
		string BinaryName { get; }
		string WindowTitle { get; }
		CompositeIcon Icon { get; }
		
		void InstantiateIntoWindow(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args);
	}
}