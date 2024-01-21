#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Scripting.Parsing;
using Modding;
using Modules;
using OS.Devices;
using UnityEngine;

namespace Core.Scripting
{
	[CreateAssetMenu(menuName = "ScriptableObject/Shell Scripting/Script Execution Context")]
	public class ScriptExecutionContext : 
		ScriptableObject,
		IScriptExecutionContext
	{
		[SerializeField]
		private ScriptCommandProvider contextCommandProvider = null!;

		private readonly Dictionary<string, IScriptCommand> contextCommandCache = new Dictionary<string, IScriptCommand>();
		private bool isCacheReady;
		
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
		public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			callSite ??= this;
			
			var system = SystemModule.GetSystemModule();
			IGameContext gameContext = system.Context;
			
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
			
			// Then search for script assets in /Resources.
			// This may result in a context switch.
			var scriptAsset = Resources.Load<ShellScriptAsset>(name);
			if (scriptAsset != null)
			{
				await scriptAsset.ExecuteAsync(console);
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
				Debug.LogError($"Redirecting console I/O to files is not supported inside Socially Distant game scripts, quests, or chat encounters.");
			
			return realConsole;
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, ITextConsole console)
		{
			// Throw it as an exception.
			throw new InvalidOperationException($"Script execution error: Command {name} not found.");
		}
	}
}