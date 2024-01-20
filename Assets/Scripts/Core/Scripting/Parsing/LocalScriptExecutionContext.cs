#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using OS.Devices;

namespace Core.Scripting.Parsing
{
	public class LocalScriptExecutionContext : IScriptExecutionContext
	{
		private readonly IScriptExecutionContext underlyingContext;
		private readonly Dictionary<string, string> localVariables = new Dictionary<string, string>();
		private readonly Dictionary<string, ScriptFunction> functions = new Dictionary<string, ScriptFunction>();
		
		public LocalScriptExecutionContext(IScriptExecutionContext underlyingContext)
		{
			this.underlyingContext = underlyingContext;
		}

		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			if (!localVariables.TryGetValue(variableName, out string result))
				result = underlyingContext.GetVariableValue(variableName);

			return result;
		}

		/// <inheritdoc />
		public void SetVariableValue(string variableName, string value)
		{
			localVariables[variableName] = value;
		}

		/// <inheritdoc />
		public async Task<bool> TryExecuteCommandAsync(string name, string[] args, ITextConsole console)
		{
			// Always try functions first.
			if (functions.TryGetValue(name, out ScriptFunction function))
				return await function.ExecuteAsync(name, args, console);
			
			return await underlyingContext.TryExecuteCommandAsync(name, args, console);
		}

		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			return underlyingContext.OpenFileConsole(realConsole, filePath, mode);
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, ITextConsole console)
		{
			underlyingContext.HandleCommandNotFound(name, console);
		}
	}
}