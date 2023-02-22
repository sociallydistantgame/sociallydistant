#nullable enable
namespace UI.Shell
{
	public interface ICommandShell
	{
		string GetVariableValue(string name);
	}
}