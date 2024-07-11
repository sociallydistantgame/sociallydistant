#nullable enable

using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core.Scripting
{
	public class ShellScriptAsset
	{
		
		private ScriptExecutionContext context = null!;
		private string scriptText = string.Empty;

		public async Task ExecuteAsync(ITextConsole console)
		{
			if (context == null)
				throw new InvalidOperationException("You cannot run this script - it has no execution context.");
			
			var shell =  new InteractiveShell(context);
			var process = new HypervisorProcess();
			
			shell.Setup(console);

			await shell.RunScript(this.scriptText);
		}
		
#if UNITY_EDITOR
		public void SetScriptContext(ScriptExecutionContext ctx)
		{
			this.context = ctx;
		}
		
		public void SetScriptText(string scriptText)
		{
			this.scriptText = scriptText;
		}
		
#endif
	}
}