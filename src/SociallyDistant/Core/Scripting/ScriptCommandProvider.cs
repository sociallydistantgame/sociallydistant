#nullable enable
namespace SociallyDistant.Core.Scripting
{
	public abstract class ScriptCommandProvider
	{
		public abstract IEnumerable<ScriptContextCommand> ContextCommands { get; }
	}
}