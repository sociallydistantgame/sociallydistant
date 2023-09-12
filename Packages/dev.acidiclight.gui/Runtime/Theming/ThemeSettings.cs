using UnityEngine;

namespace AcidicGui.Theming
{
	public abstract class ThemeSettings : ScriptableObject
	{
		[Tooltip("A color to use as an accent color if a UI element requests accent colors but the current theme's engine doesn't support accent colors.")]
		[SerializeField]
		private Color fallbackAccentColor;
		
		protected internal abstract ITheme PreviewTheme { get; }
		
		#if UNITY_EDITOR
		public ITheme GetEditorPreviewTheme()
		{
			return PreviewTheme;
		}
		#endif
	}
}