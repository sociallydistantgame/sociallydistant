#nullable enable
using System;
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
		public async Task<bool> TryExecuteCommandAsync(string name, string[] args, ITextConsole console)
		{
			var system = SystemModule.GetSystemModule();
			IGameContext gameContext = system.Context;
			
			// Search for global commands first
			IScriptCommand? globalCommand = gameContext.ScriptSystem.GetGlobalCommand(name);
			if (globalCommand != null)
			{
				// Create a local context so we're not passing this ScriptableObject around directly,
				// and execute the command in it.
				var localContext = new LocalScriptExecutionContext(this);
				await globalCommand.ExecuteAsync(localContext, console, name, args);
				return true;
			}
			
			// Then search for script assets in /Resources.
			// This may result in a context switch.
			var scriptAsset = Resources.Load<ShellScriptAsset>(name);
			if (scriptAsset != null)
			{
				await scriptAsset.ExecuteAsync(console);
				return true;
			}
			
			// TODO: Context-local commands.
			return false;
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