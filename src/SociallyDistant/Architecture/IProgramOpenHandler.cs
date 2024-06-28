#nullable enable
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.Architecture
{
	public interface IProgramOpenHandler
	{
		void OnProgramOpen(ISystemProcess process, IContentPanel window, ITextConsole console, string[] args);
	}
}