#nullable enable
using System.Collections.Generic;
using Core.Scripting.WorldCommands;
using UnityEngine;

namespace Core.Scripting
{
	[CreateAssetMenu(menuName = "ScriptableObject/Shell Scripting/Context CommandProviders/World Commands")]
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