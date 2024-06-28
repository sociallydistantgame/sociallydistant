#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting
{
	public sealed class UserScriptExecutionContext : IScriptExecutionContext
	{
		private readonly Dictionary<string, string> variables = new(0);
		private readonly Dictionary<string, IScriptCommand> builtIns = new(0);
		private readonly ScriptFunctionManager functions = new();
		private readonly ScriptModuleManager moduleManager;

		public ScriptModuleManager ModuleManager => moduleManager;
		

		public event Action<string, ITextConsole>? OnCommandNotFound;
		
		/// <inheritdoc />
		public string Title { get; set; } = "User script";

		public UserScriptExecutionContext()
		{
			moduleManager = new ScriptModuleManager();
		}
		
		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			if (!variables.TryGetValue(variableName, out string value))
				return string.Empty;

			return value;
		}

		/// <inheritdoc />
		public void SetVariableValue(string variableName, string value)
		{
			variables[variableName] = value;
		}

		/// <inheritdoc />
		public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			int? functionResult = await functions.CallFunction(name, args, console, callSite ?? this);
			if (functionResult != null)
				return functionResult;

			int? moduleResult = await ModuleManager.TryExecuteFunction(name, args, console, callSite ?? this);
			if (moduleResult != null)
				return moduleResult;
			
			if (name == "export")
			{
				// ignored for now
				return 0;
			}
			
			if (builtIns.TryGetValue(name, out IScriptCommand command))
			{
				await command.ExecuteAsync(callSite ?? this, console, name, args);
				return 0;
			}

			if (callSite != null && callSite != this)
				return await callSite.TryExecuteCommandAsync(name, args, console, this);
			
			return null;
		}

		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			return realConsole;
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, string[] args, ITextConsole console)
		{
			if (OnCommandNotFound != null)
				OnCommandNotFound(name, console);
			else
				throw new InvalidOperationException($"{Title}: {name}: Command not found");
		}

		/// <inheritdoc />
		public void DeclareFunction(string name, IScriptFunction body)
		{
			functions.DeclareFunction(name, body);
		}
	}
}