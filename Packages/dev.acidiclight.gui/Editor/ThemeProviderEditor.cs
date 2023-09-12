#nullable enable
using System;
using AcidicGui.Theming;
using UnityEditor;

namespace AcidicGui.Editor.Editor
{
	[CustomEditor(typeof(ThemeProviderComponent), true)]
	public class ThemeProviderEditor : UnityEditor.Editor
	{
		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (target is not ThemeProviderComponent provider)
				return;

			try
			{
				ITheme theme = provider.GetCurrentTheme();
				EditorGUILayout.HelpBox("This theme provider has located a compatible theme to use for themed UI elements!", MessageType.Info);
			}
			catch (Exception ex)
			{
				EditorGUILayout.HelpBox(ex.Message, MessageType.Error);
			}
		}
	}
}