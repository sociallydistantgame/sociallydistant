#nullable enable
using UI.Windowing;

namespace UI.Shell
{
	public interface IDesktop
	{
		IWorkspaceDefinition CurrentWorkspace { get; }
	}
}