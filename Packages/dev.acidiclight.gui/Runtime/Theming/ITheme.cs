namespace AcidicGui.Theming
{
	/// <summary>
	///		Interface for a theme that uses a specified theme engine.
	/// </summary>
	/// <typeparam name="TThemeEngine">Specify the theme engine this theme uses. Must implement IThemeEngine.</typeparam>
	public interface ITheme<TThemeEngine> :
		ITheme
		where TThemeEngine : IThemeEngine
	{
		
	}

	/// <summary>
	///		Common interface for all themes.
	/// </summary>
	public interface ITheme
	{
		
	}
}