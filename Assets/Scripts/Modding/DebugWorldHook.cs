#nullable enable
using System.Threading.Tasks;
using Core.Scripting;
using Modules;

namespace Modding
{
	public sealed class DebugWorldHook : IHookListener
	{
		/// <inheritdoc />
		public Task ReceiveHookAsync(IGameContext game)
		{
			if (game.IsDebugWorld)
			{
				if (!game.WorldManager.World.WorldFlags.Contains("DEBUG"))
					game.WorldManager.World.WorldFlags.Add("DEBUG");
			}
			else
			{
				if (game.WorldManager.World.WorldFlags.Contains("DEBUG"))
					game.WorldManager.World.WorldFlags.Remove("DEBUG");
			}
			
			return Task.CompletedTask;
		}
	}
}