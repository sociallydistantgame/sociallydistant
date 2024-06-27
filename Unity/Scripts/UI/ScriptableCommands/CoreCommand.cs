#nullable enable
using System.Threading.Tasks;
using Core;
using OS.Devices;
using OS.Tasks;

namespace UI.ScriptableCommands
{
	public abstract class CoreCommand :
		ICommandTask,
		INamedAsset
	{
		/// <inheritdoc />
		public async Task Main(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			var consoleWrapper = new ConsoleWrapper(console);

			await OnRun(consoleWrapper, arguments);
			
			process.Kill();
		}

		protected abstract Task OnRun(ConsoleWrapper console, string[] args);

		/// <inheritdoc />
		public abstract string Name { get; }
	}
}