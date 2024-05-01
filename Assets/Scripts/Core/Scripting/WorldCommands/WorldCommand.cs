#nullable enable

using System.Threading.Tasks;
using Modding;
using Modules;
using OS.Devices;

namespace Core.Scripting.WorldCommands
{
	public abstract class WorldCommand : IScriptCommand
	{
		/// <inheritdoc />
		public async Task ExecuteAsync(IScriptExecutionContext context, ITextConsole console, string name, string[] args)
		{
			var systemModule = SystemModule.GetSystemModule();

			IGameContext game = systemModule.Context;
			IWorldManager worldManager = game.WorldManager;

			await OnExecute(worldManager, context, console, name, args);
		}

		protected abstract Task OnExecute(
			IWorldManager worldManager,
			IScriptExecutionContext context,
			ITextConsole console,
			string name,
			string[] args
		);
	}
}