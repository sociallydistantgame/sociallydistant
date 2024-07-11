#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting.Parsing
{
	public class LocalScriptExecutionContext : IScriptExecutionContext
	{
		private readonly IScriptExecutionContext underlyingContext;
		private readonly Dictionary<string, string> localVariables = new Dictionary<string, string>();
		private readonly Dictionary<string, IScriptFunction> functions = new Dictionary<string, IScriptFunction>();
		private readonly Stack<FunctionFrame> functionFrames = new Stack<FunctionFrame>();
		private readonly bool leakVariables;

		public FunctionFrame? CurrentFrame => functionFrames.Count > 0 ? functionFrames.Peek() : null;
		
		public LocalScriptExecutionContext(IScriptExecutionContext underlyingContext, bool leakVariables = false)
		{
			this.underlyingContext = underlyingContext;
			this.leakVariables = leakVariables;
		}

		/// <inheritdoc />
		public string Title => this.underlyingContext.Title;

		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			if (CurrentFrame != null && CurrentFrame.TryGetVariable(variableName, out string frameValue))
				return frameValue;
			
			if (!localVariables.TryGetValue(variableName, out string result))
				result = underlyingContext.GetVariableValue(variableName);

			return result;
		}

		/// <inheritdoc />
		public void SetVariableValue(string variableName, string value)
		{
			if (leakVariables)
			{
				this.underlyingContext.SetVariableValue(variableName, value);
				return;
			}
			
			localVariables[variableName] = value;
		}

		/// <inheritdoc />
		public async Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			callSite ??= this;
			
			// Always try functions first.
			if (functions.TryGetValue(name, out IScriptFunction function))
			{
				functionFrames.Push(new FunctionFrame());
				
				// Prepare function parameters
				// $0 is always the function name
				// $1-$n are arguments
				CurrentFrame?.SetVariableValue("0", name);
				for (var i = 0; i < args.Length; i++)
					CurrentFrame?.SetVariableValue(i.ToString(), args[i]);
				
				int result = await function.ExecuteAsync(name, args, console, callSite);

				functionFrames.Pop();
				
				return result;
			}

			return await underlyingContext.TryExecuteCommandAsync(name, args, console, callSite);
		}

		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			return underlyingContext.OpenFileConsole(realConsole, filePath, mode);
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, string[] args, ITextConsole console)
		{
			underlyingContext.HandleCommandNotFound(name, args, console);
		}

		/// <inheritdoc />
		public void DeclareFunction(string name, IScriptFunction body)
		{
			this.functions[name] = body;
		}
	}

	public class FunctionFrame
	{
		private readonly Dictionary<string, string> frameValues = new Dictionary<string, string>();

		public bool TryGetVariable(string variableName, out string value)
		{
			return frameValues.TryGetValue(variableName, out value);
		}

		public void SetVariableValue(string variableName, string value)
		{
			this.frameValues[variableName] = value;
		}
	}
}