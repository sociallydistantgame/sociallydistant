#nullable enable

using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting.WorldCommands
{
	public abstract class WorldCommand : IScriptCommand
	{
		/// <inheritdoc />
		public async Task ExecuteAsync(IScriptExecutionContext context, ITextConsole console, string name, string[] args)
		{
			IGameContext game = Application.Instance.Context;
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