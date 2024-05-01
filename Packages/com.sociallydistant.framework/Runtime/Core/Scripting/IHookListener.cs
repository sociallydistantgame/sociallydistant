#nullable enable
using System.Threading.Tasks;
using Modules;

namespace Core.Scripting
{
	public interface IHookListener
	{
		Task ReceiveHookAsync(IGameContext game);
	}
}