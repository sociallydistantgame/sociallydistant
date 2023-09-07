using System.Collections.Generic;

namespace Shell.Windowing
{
	public interface IWorkspaceDefinition
	{
		string Name { get; set; }
		
		IReadOnlyList<IWindow> WindowList { get; }

		IWindow CreateWindow(string title);
	}
}