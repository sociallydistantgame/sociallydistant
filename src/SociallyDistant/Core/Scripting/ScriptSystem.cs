#nullable enable

using System.Diagnostics;
using Serilog;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.GamePlatform;

namespace SociallyDistant.Core.Scripting
{
	public class ScriptSystem : IScriptSystem
	{
		private readonly Dictionary<string, List<IHookListener>> hooks = new Dictionary<string, List<IHookListener>>();
		private readonly Dictionary<string, IScriptCommand> globalCommands = new Dictionary<string, IScriptCommand>();
		private readonly IGameContext game;

		internal ScriptSystem(IGameContext game)
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

			await command.ExecuteAsync(context, console ?? new UnityTextConsole(), name, args);
		}

		/// <inheritdoc />
		public async Task RunHookAsync(string hookName)
		{
			if (!hooks.TryGetValue(hookName, out List<IHookListener> listeners))
			{
				Log.Warning($"Script hook {hookName} has no registered listeners.");
				return;
			}

			foreach (IHookListener listener in listeners)
			{
				await listener.ReceiveHookAsync(this.game);
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