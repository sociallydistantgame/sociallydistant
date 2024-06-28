#nullable enable
namespace SociallyDistant.Core.Modules
{
	public interface IModuleManager
	{
		IEnumerable<GameModule> Modules { get; }
	}
}