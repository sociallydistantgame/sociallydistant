#nullable enable
namespace SociallyDistant.Core.Core.Scripting
{
	public interface ICommandShell
	{
		string GetVariableValue(string name);
		void SetVariableValue(string name, string newValue);
	}
}