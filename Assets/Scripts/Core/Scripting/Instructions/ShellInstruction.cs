#nullable enable

using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting.Instructions
{
	public abstract class ShellInstruction
	{
		public abstract Task RunAsync(ITextConsole console);
	}
}