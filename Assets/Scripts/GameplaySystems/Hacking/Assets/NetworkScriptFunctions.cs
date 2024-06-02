#nullable enable
using Core;
using Core.Scripting;

namespace GameplaySystems.Hacking.Assets
{
	public sealed class NetworkScriptFunctions : ScriptModule
	{
		private readonly IWorldManager worldManager;

		public NetworkScriptFunctions(IWorldManager worldManager)
		{
			this.worldManager = worldManager;
		}
	}
}