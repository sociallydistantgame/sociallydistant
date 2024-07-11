#nullable enable
using Serilog;
using SociallyDistant.Core.Core.Scripting.Parsing;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting
{
	public class ScriptExecutionContext : IScriptExecutionContext
	{
		
		private ScriptCommandProvider contextCommandProvider = null!;

		private readonly Dictionary<string, IScriptCommand> contextCommandCache = new Dictionary<string, IScriptCommand>();
		private readonly ScriptFunctionManager functions = new();
		private bool isCacheReady;

		/// <inheritdoc />
		public string Title => "unknown";

		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			return string.Empty;
		}

		/// <inheritdoc />
		public void SetVariableValue(string variableName, string value)
		{
		}

		/// <inheritdoc />
		public virtual async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			callSite ??= this;

			int? functionResult = await functions.CallFunction(name, args, console, callSite);
			if (functionResult != null)
				return functionResult;
			
			IGameContext gameContext = Application.Instance.Context;
			
			// Search for global commands first
			IScriptCommand? globalCommand = gameContext.ScriptSystem.GetGlobalCommand(name);
			if (globalCommand != null)
			{
				// Create a local context so we're not passing this ScriptableObject around directly,
				// and execute the command in it.
				var localContext = new LocalScriptExecutionContext(callSite);
				await globalCommand.ExecuteAsync(localContext, console, name, args);
				return 0;
			}
			
			// If we're in Unity Editor, we do not cache script commands. Because if we do,
			// then the cache will persist across multiple Play Mode sessions and we don't want that.
			#if UNITY_EDITOR
			isCacheReady = false;
			#endif
			
			if (!isCacheReady)
			{
				this.contextCommandCache.Clear();
				if (this.contextCommandProvider != null)
				{
					foreach (ScriptContextCommand? command in contextCommandProvider.ContextCommands)
						contextCommandCache[command.Name] = command.ScriptCommand;
				}

				this.isCacheReady = true;
			}

			if (this.contextCommandCache.TryGetValue(name, out IScriptCommand scriptCommand))
			{
				var localContext = new LocalScriptExecutionContext(callSite);
				await scriptCommand.ExecuteAsync(localContext, console, name, args);
				return 0;
			}
			
			return null;
		}

		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			if (mode != FileRedirectionType.None)
				Log.Error($"Redirecting console I/O to files is not supported inside Socially Distant game scripts, quests, or chat encounters.");
			
			return realConsole;
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, string[] args, ITextConsole console)
		{
			// Throw it as an exception.
			throw new InvalidOperationException($"Script execution error: Command {name} not found.");
		}

		/// <inheritdoc />
		public void DeclareFunction(string name, IScriptFunction body)
		{
			functions.DeclareFunction(name, body);
		}
	}
}