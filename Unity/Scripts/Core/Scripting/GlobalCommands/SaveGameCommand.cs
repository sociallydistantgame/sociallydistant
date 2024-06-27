#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using Modules;
using OS.Devices;

namespace Core.Scripting.GlobalCommands
{
	public class SaveGameCommand : IScriptCommand
	{
		private readonly IGameContext game;

		public SaveGameCommand(IGameContext game)
		{
			this.game = game;
		}

		/// <inheritdoc />
		public async Task ExecuteAsync(IScriptExecutionContext context, ITextConsole console, string name, string[] args)
		{
			bool silent = args.Contains("-s");

			await game.SaveCurrentGame(silent);
		}
	}
}