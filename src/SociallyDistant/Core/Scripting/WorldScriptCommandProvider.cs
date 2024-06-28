#nullable enable
using SociallyDistant.Core.Scripting.WorldCommands;

namespace SociallyDistant.Core.Scripting
{
	public sealed class WorldScriptCommandProvider : ScriptCommandProvider
	{
		/// <inheritdoc />
		public override IEnumerable<ScriptContextCommand> ContextCommands
		{
			get
			{
				yield return new ScriptContextCommand("spawnisp", new SpawnIspCommand());
				yield return new ScriptContextCommand("setplayerisp", new SetPlayerIspCommand());
			}
		}
	}
}