using UnityEngine;

namespace AcidicGui.Theming
{
	public abstract class ScriptableThemeProvider<TScriptableTheme, TThemeEngine> :
		ScriptableObject,
		IThemeProvider<TScriptableTheme, TThemeEngine>
		where TScriptableTheme : ScriptableTheme<TThemeEngine>
		where TThemeEngine : IThemeEngine
	{
		/// <inheritdoc />
		public abstract TScriptableTheme CurrentTheme { get; }

		/// <inheritdoc />
		public ITheme GetCurrentTheme()
		{
			return this.CurrentTheme;
		}
	}
}