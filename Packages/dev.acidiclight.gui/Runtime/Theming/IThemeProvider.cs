namespace AcidicGui.Theming
{
	public interface IThemeProvider
	{
		ITheme GetCurrentTheme();
	}
	
	public interface IThemeProvider<TTheme, TThemeEngine> :
		IThemeProvider
		where TTheme : ITheme<TThemeEngine>
		where TThemeEngine : IThemeEngine
	{
		TTheme CurrentTheme { get; }
	}
}