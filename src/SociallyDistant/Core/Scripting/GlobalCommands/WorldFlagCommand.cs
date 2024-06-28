#nullable enable
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Scripting.GlobalCommands
{
	public class WorldFlagCommand : IScriptCommand
	{
		private readonly IWorldManager worldManager;

		private readonly string usage = @"Usage:
	worldflag set <flag>                               - Checks if the given world flag is set in the current world. The result will be returned as a 0 or 1 value in the command's exit status.
	worldflag unsert <flag>                            - Unsets the specified world flag in the current world.
	worldflag get <flag>                               - Sets the specified world flag in the current world.
	worldflag run-if-set <flag> <command> [args...]    - Runs the specified command with the given arguments if the given world flag is set.
	worldflag run-if-unset <flag> <command> [args...]  - Runs the specified command with the given arguments if the given world flag isn't set.";  
		internal WorldFlagCommand(IWorldManager worldManager)
		{
			this.worldManager = worldManager;
		}

		/// <inheritdoc />
		public async Task ExecuteAsync(IScriptExecutionContext context, ITextConsole console, string name, string[] args)
		{
			if (args.Length < 2)
			{
				console.WriteText(usage + Environment.NewLine);
				return;
			}

			string method = args[0];
			string flagName = args[1];
			string? commandName = null;

			switch (method)
			{
				case "get":
					// TODO
					break;
				case "set":
					if (!worldManager.World.WorldFlags.Contains(flagName))
						worldManager.World.WorldFlags.Add(flagName);
					
					break;
				case "unset":
					if (worldManager.World.WorldFlags.Contains(flagName))
						worldManager.World.WorldFlags.Remove(flagName);
					
					break;
				case "run-if-set":
					if (args.Length < 3)
						goto default;

					if (!worldManager.World.WorldFlags.Contains(flagName))
						break;
					
					commandName = args[2];

					if (!(await context.TryExecuteCommandAsync(commandName, args.Skip(3).ToArray(), console)).HasValue)
						context.HandleCommandNotFound(commandName, args, console);
					
					break;
				case "run-if-unset" :
					if (args.Length < 3)
						goto default;

					if (worldManager.World.WorldFlags.Contains(flagName))
						break;
					
					commandName = args[2];
					
					if (!(await context.TryExecuteCommandAsync(commandName, args.Skip(3).ToArray(), console)).HasValue)
						context.HandleCommandNotFound(commandName, args, console);

					break;
				default:
					console.WriteText(usage + Environment.NewLine);
					return;
			}
		}
	}
}