#nullable enable
using System.Threading.Tasks;
using OS.Devices;

namespace OS.Tasks
{
	public interface ICommandTask
	{
		Task Main(ISystemProcess process, ITextConsole console, string[] arguments);
	}
}