#nullable enable

using AcidicGui.Theming;
using UnityEditor;
using UnityEngine;

namespace AcidicGui.Editor.Editor
{
	[CustomEditor(typeof(ThemeSettings), true)]
	public class ThemeSettingsEditor : UnityEditor.Editor
	{
		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (target is ThemeSettings themeSettings)
				ValidateAsset(themeSettings);
		}

		public static void ValidateAsset(ThemeSettings themeSettings)
		{
			if (themeSettings.GetEditorPreviewTheme() == null)
				EditorGUILayout.HelpBox("Please create and assign a compatible theme asset to uise as the editor preview theme and the project's default theme.", MessageType.Error);
		}
	}
}