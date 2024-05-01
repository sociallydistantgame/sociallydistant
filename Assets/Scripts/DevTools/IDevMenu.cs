namespace DevTools
{
	public interface IDevMenu
	{
		string Name { get; }

		void OnMenuGUI(DeveloperMenu devMenu);
	}
}