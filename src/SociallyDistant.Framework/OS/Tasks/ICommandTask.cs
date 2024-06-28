#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.OS.Tasks
{
	public interface ICommandTask
	{
		Task Main(ISystemProcess process, ITextConsole console, string[] arguments);
	}
}