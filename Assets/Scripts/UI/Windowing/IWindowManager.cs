#nullable enable

using Architecture;

namespace UI.Windowing
{
	public interface IWindowManager<TWorkspace, TWorkspaceCreationParameters>
		where TWorkspace : IWorkspaceDefinition
	{
		ObservableList<TWorkspace> WorkspaceList { get; }
		
		TWorkspace FallbackWorkspace { get; }

		TWorkspace DefineWorkspace(TWorkspaceCreationParameters? creationParams);
		
		IMessageDialog CreateMessageDialog(string title, IWindow? parent = null!);
	}
}