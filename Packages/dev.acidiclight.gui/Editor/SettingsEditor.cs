#nullable enable
using UnityEditor;

namespace AcidicGui.Editor.Editor
{
	[CustomEditor(typeof(AcidicGUISettings))]
	public class SettingsEditor : UnityEditor.Editor
	{
		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (this.target is AcidicGUISettings settings)
			{
				ValidateSettings(settings);
			}
		}

		public static void ValidateSettings(AcidicGUISettings settings)
		{
			if (settings.ThemeSettings == null)
				EditorGUILayout.HelpBox("Please create and assign a Theme Settings asset to enable UI theming.", MessageType.Error);
		}
	}
}