#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting
{
	public sealed class ScriptFunctionManager
	{
		private readonly Dictionary<string, IScriptFunction> declaredFunctions = new();

		public async Task<int?> CallFunction(string name, string[] args, ITextConsole console, IScriptExecutionContext context)
		{
			if (!declaredFunctions.TryGetValue(name, out IScriptFunction function))
				return null;

			return await function.ExecuteAsync(name, args, console, context);
		}
		
		public void DeclareFunction(string name, IScriptFunction function)
		{
			this.declaredFunctions[name] = function;
		}
	}
}