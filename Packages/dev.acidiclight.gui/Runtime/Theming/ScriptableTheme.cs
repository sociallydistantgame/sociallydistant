using UnityEngine;

namespace AcidicGui.Theming
{
	public abstract class ScriptableTheme<TThemeEngine> : 
		ScriptableObject,
		ITheme<TThemeEngine>
		where TThemeEngine : IThemeEngine
	{
		
	}
}