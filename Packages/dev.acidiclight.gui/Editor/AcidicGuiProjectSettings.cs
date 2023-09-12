#nullable enable

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AcidicGui.Editor.Editor
{
	public static class AcidicGuiProjectSettings
	{
		private static readonly string RootPath = "Project/AcidicGUISettings";
        
		[SettingsProvider]
		private static SettingsProvider GetProjectSettings()
		{
			var settingsGuid = string.Empty;
			AcidicGUISettings? settingsAsset = AcidicGUISettings.GetSettings();

			if (settingsAsset != null)
				AssetDatabase.TryGetGUIDAndLocalFileIdentifier(settingsAsset, out settingsGuid, out long _);
			
			
			var provider = new SettingsProvider(RootPath, SettingsScope.Project)
			{
				label = "Acidic GUI",
				guiHandler = (searchContext) =>
				{
					var newObject = EditorGUILayout.ObjectField("Settings asset", settingsAsset, typeof(AcidicGUISettings), allowSceneObjects: false) as AcidicGUISettings;
					if (newObject != settingsAsset)
					{
						settingsAsset = newObject;
						AcidicGUISettings.SaveSettingsGuid(settingsAsset, out settingsGuid);
					}

					if (settingsAsset == null)
					{
						EditorGUILayout.HelpBox("Please create and assign an Acidic GUI Settings asset.", MessageType.Error);
						return;
					}
					
					var settings = new SerializedObject(settingsAsset);
                    
					SerializedProperty iterator = settings.GetIterator();

					if (iterator.NextVisible(true))
					{
						while (iterator.NextVisible(false))
						{
							EditorGUILayout.PropertyField(iterator);
						}
					}

					SettingsEditor.ValidateSettings(settingsAsset);

					if (settings.hasModifiedProperties)
						settings.ApplyModifiedProperties();
				}
			};

			return provider;
		}

		[SettingsProvider]
		private static SettingsProvider GetThemeSettingsProvider()
		{
			AcidicGUISettings? acidicSettings = AcidicGUISettings.GetSettings();

			var provider = new SettingsProvider(RootPath + "/Theme Settings", SettingsScope.Project)
			{
				label = "Theme Settings",
				guiHandler = (searchContext) =>
				{
					if (acidicSettings == null)
					{
						EditorGUILayout.HelpBox("The Acidic GUI Settings asset is missing. Please create one and assign it within the Acidic GUI project settings.", MessageType.Error);
						return;
					}

					if (acidicSettings.ThemeSettings == null)
					{
						EditorGUILayout.HelpBox("Themes are not supported by this project. Please create and assign a Theme Settings asset in the Acidic GUI project settings.", MessageType.Error);
						return;
					}

					var themeSettingsObject = new SerializedObject(acidicSettings.ThemeSettings);

					SerializedProperty property = themeSettingsObject.GetIterator();

					if (property.NextVisible(true))
					{
						while (property.NextVisible(false))
						{
							EditorGUILayout.PropertyField(property);
						}
					}
					
					ThemeSettingsEditor.ValidateAsset(acidicSettings.ThemeSettings);
                    
					if (themeSettingsObject.hasModifiedProperties)
						themeSettingsObject.ApplyModifiedProperties();
				}
			};

			return provider;
		}
	}
}