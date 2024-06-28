#nullable enable
using SociallyDistant.Core.Modules;

namespace SociallyDistant.Core.Core.Scripting
{
	public interface IHookListener
	{
		Task ReceiveHookAsync(IGameContext game);
	}
}