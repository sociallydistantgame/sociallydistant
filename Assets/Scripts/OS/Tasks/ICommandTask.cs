#nullable enable
using OS.Devices;

namespace OS.Tasks
{
	public interface ICommandTask
	{
		void Main(ISystemProcess process, ITextConsole console, string[] arguments);
	}
}