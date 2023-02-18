namespace UI.Windowing
{
	public interface IClientWorkspaceDefinition<TWindowWithClient, TWindowClient> : IWorkspaceDefinition
		where TWindowWithClient : IWindowWithClient<TWindowClient>, IWindow
	{
		TWindowWithClient CreateWindow(string title, TWindowClient? client = default);
	}
}