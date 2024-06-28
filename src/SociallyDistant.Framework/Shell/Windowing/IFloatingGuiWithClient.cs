#nullable enable
namespace SociallyDistant.Core.Shell.Windowing
{
	public interface IFloatingGuiWithClient<TClient> : IFloatingGui, IWindowWithClient<TClient>
	{
		
	}
}