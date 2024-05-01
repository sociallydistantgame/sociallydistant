#nullable enable
using System.Threading.Tasks;
using OS.Devices;
using OS.Tasks;
using UnityEngine;

namespace Core.Scripting
{
	public sealed class OperatingSystemScript : 
		ScriptableObject,
		IShellScript
	{
		[SerializeField]
		[TextArea]
		private string text = string.Empty;

		public async Task Run(ISystemProcess process, string[] args, ITextConsole console)
		{
			var context = new OperatingSystemExecutionContext(process);
			var shell = new InteractiveShell(context);

			for (var i = 0; i < args.Length; i++)
			{
				context.SetVariableValue((i + 1).ToString(), args[i]);
			}
			
			shell.Setup(console);

			await shell.RunScript(text);
		}
		
#if UNITY_EDITOR

		public void SetScriptText(string text)
		{
			this.text = text;
		}
		
#endif
	}
}