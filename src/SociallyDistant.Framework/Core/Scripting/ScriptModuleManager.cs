#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting
{
	public sealed class ScriptModuleManager
	{
		private readonly List<ScriptModule> modules = new();

		public T RegisterModule<T>() where T : ScriptModule, new()
		{
			var module = new T();
			RegisterModule(module);
			return module;
		}

		public void RegisterModule(ScriptModule module)
		{
			this.modules.Add(module);
		}

		public void UnregisterModule(ScriptModule module)
		{
			this.modules.Remove(module);
		}
		
		public async Task<int?> TryExecuteFunction(string name, string[] args, ITextConsole console, IScriptExecutionContext context)
		{
			foreach (ScriptModule module in modules)
			{
				int? result = await module.TryExecuteFunction(name, args, console, context);
				if (result != null)
					return result;
			}

			return null;
		}
	}
}