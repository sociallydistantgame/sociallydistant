using System;

namespace AcidicGui.Theming
{
	public interface IObservableThemeProvider<TTheme, TThemeEngine> :
		IThemeProvider<TTheme, TThemeEngine>
		where TTheme : ITheme<TThemeEngine>
		where TThemeEngine : IThemeEngine
	{
		IDisposable ObserveCurrentTheme(Action<TTheme> themeChangedCallback);
	}
}