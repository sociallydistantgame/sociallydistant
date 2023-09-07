#nullable enable

using System.Collections.Generic;

namespace Shell.Windowing
{
	public interface IWindowManager<TWorkspace, TWorkspaceCreationParameters>
		where TWorkspace : IWorkspaceDefinition
	{
		IReadOnlyList<TWorkspace> WorkspaceList { get; }
		
		TWorkspace FallbackWorkspace { get; }

		TWorkspace DefineWorkspace(TWorkspaceCreationParameters? creationParams);
		
		IMessageDialog CreateMessageDialog(string title, IWindow? parent = null!);
	}
}