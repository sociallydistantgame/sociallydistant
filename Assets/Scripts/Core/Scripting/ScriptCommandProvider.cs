#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripting
{
	public abstract class ScriptCommandProvider : ScriptableObject
	{
		public abstract IEnumerable<ScriptContextCommand> ContextCommands { get; }
	}
}