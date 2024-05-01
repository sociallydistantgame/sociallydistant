#nullable enable
namespace Core.Scripting
{
	public class ScriptContextCommand
	{
		public string Name { get; }
		public IScriptCommand ScriptCommand { get; }

		public ScriptContextCommand(string name, IScriptCommand command)
		{
			this.Name = name;
			this.ScriptCommand = command;
		}
	}
}