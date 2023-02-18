using Architecture;

namespace UI.Windowing
{
	public interface IWorkspaceDefinition<TWindowClient>
	{
		string Name { get; set; }
		
		ObservableList<IWindow<TWindowClient>> WindowList { get; }

		IWindow<TWindowClient> CreateWindow(string title, TWindowClient? client = default);
	}
}