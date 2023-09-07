#nullable enable
using Shell.Windowing;
using UI.Windowing;

namespace UI.Shell
{
	public interface IDesktop
	{
		IWorkspaceDefinition CurrentWorkspace { get; }
	}
}