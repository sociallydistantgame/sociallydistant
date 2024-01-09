#nullable enable
namespace Shell.Windowing
{
	public interface IFloatingGuiWithClient<TClient> : IFloatingGui, IWindowWithClient<TClient>
	{
		
	}
}