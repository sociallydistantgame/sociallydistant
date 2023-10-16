#nullable enable

using System;
using UI.Theming;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;

namespace Editor.CustomEditors
{
	[CustomEditor(typeof(OperatingSystemTheme))]
	public class ThemeEditor : UnityEditor.Editor
	{
		private Task? exportTask;
		
		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			if (target is not OperatingSystemTheme theme)
				return;

			if (exportTask != null)
			{
				EditorGUILayout.HelpBox("Exporting...", MessageType.Info);
				
				if (exportTask.Exception != null)
				{
					Debug.LogException(exportTask.Exception);
					exportTask = null;
				}
				else if (exportTask.IsCanceled)
					exportTask = null;
				
				return;
			}
			
			bool wasEnabled = GUI.enabled;
			GUI.enabled = true;
			if (GUILayout.Button("Export"))
			{
				string path = EditorUtility.SaveFilePanel("Export theme", Environment.CurrentDirectory, target.name, "sdtheme");
				exportTask = theme.ExportAsync(path);
			}

			GUI.enabled = wasEnabled;
			
			base.OnInspectorGUI();
		}
	}
}