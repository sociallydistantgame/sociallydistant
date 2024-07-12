#nullable enable

using Serilog;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting
{
	public class ScriptSystem : IScriptSystem
	{
		private readonly Dictionary<string, List<IHookListener>> hooks = new Dictionary<string, List<IHookListener>>();
		private readonly Dictionary<string, IScriptCommand> globalCommands = new Dictionary<string, IScriptCommand>();
		private readonly IGameContext game;

		public ScriptSystem(IGameContext game)
		{
			this.game = game;
		}

		/// <inheritdoc />
		public async Task RunCommandAsync(string name, string[] args, IScriptExecutionContext context, ITextConsole? console = null)
		{
			if (!globalCommands.TryGetValue(name, out IScriptCommand? command))
				return;

			if (command == null)
				return;

			await command.ExecuteAsync(context, console ?? new HostConsole(), name, args);
		}

		/// <inheritdoc />
		public async Task RunHookAsync(string hookName)
		{
			var scripts = game.ContentManager.GetContentOfType<HookScript>().Where(x => x.HookId == hookName).ToArray();
			
			if (!hooks.TryGetValue(hookName, out List<IHookListener>? listeners) && scripts.Length==0)
			{
				Log.Warning($"Script hook {hookName} has no registered listeners.");
				return;
			}

			foreach (HookScript script in scripts)
			{
				await script.ReceiveHookAsync(game);
			}
			
			if (listeners != null)
			{
				foreach (IHookListener listener in listeners)
				{
					await listener.ReceiveHookAsync(this.game);
				}
			}
		}

		/// <inheritdoc />
		public void RegisterHookListener(string hookName, IHookListener listener)
		{
			if (!hooks.TryGetValue(hookName, out List<IHookListener> listeners))
			{
				listeners = new List<IHookListener>();
				hooks.Add(hookName, listeners);
			}
			
			listeners.Add(listener);
		}

		/// <inheritdoc />
		public void UnregisterHookListener(string hookName, IHookListener listener)
		{
			if (!hooks.TryGetValue(hookName, out List<IHookListener> listeners))
				return;

			listeners.Remove(listener);
			if (listeners.Count == 0)
				hooks.Remove(hookName);
		}

		/// <inheritdoc />
		public void RegisterGlobalCommand(string commandName, IScriptCommand command)
		{
			globalCommands[commandName] = command;
		}

		/// <inheritdoc />
		public void UnregisterGlobalCommand(string commandName)
		{
			if (globalCommands.ContainsKey(commandName))
				globalCommands.Remove(commandName);
		}

		/// <inheritdoc />
		public IScriptCommand? GetGlobalCommand(string commandName)
		{
			if (globalCommands.ContainsKey(commandName))
				return globalCommands[commandName];

			return null;
		}
	}
}