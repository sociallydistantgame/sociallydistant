#nullable enable

using Architecture;

namespace UI.Windowing
{
	public interface IWindowManager<TWorkspace, TWindowClient, TWorkspaceCreationParameters>
		where TWorkspace : IWorkspaceDefinition<TWindowClient>
	{
		ObservableList<TWorkspace> WorkspaceList { get; }
		
		TWorkspace FallbackWorkspace { get; }

		TWorkspace DefineWorkspace(TWorkspaceCreationParameters? creationParams);
	}
}