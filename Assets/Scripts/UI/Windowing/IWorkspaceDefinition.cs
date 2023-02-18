using Architecture;

namespace UI.Windowing
{
	public interface IWorkspaceDefinition
	{
		string Name { get; set; }
		
		ObservableList<IWindow> WindowList { get; }

		IWindow CreateWindow(string title);
	}
}