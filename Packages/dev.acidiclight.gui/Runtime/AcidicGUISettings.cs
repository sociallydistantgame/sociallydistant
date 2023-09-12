#nullable enable

using System;
using AcidicGui.Theming;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace AcidicGui
{
	[CreateAssetMenu(menuName = "Acidic GUI/Acidic GUI Settings")]
	public class AcidicGUISettings : ScriptableObject
	{
		private static AcidicGUISettings? cached;
		
		[Header("Theming")]
		[SerializeField]
		private ThemeSettings themeSettings = null!;
        
		public ThemeSettings ThemeSettings => themeSettings;

		#region Editor-only code for ProjectSettings
#if UNITY_EDITOR
		private static readonly string SettingsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath)!, "ProjectSettings", "AcidicGUI.json");

		[Serializable]
		private class AcidicGuiJson
		{
			public string settingsAssetGuid = string.Empty;
		}
		
		public static void SaveSettingsGuid(AcidicGUISettings? settings, out string guid)
		{
			var jsonObject = new AcidicGuiJson();

			if (settings != null)
			{
				AssetDatabase.TryGetGUIDAndLocalFileIdentifier(settings, out jsonObject.settingsAssetGuid, out long _);
			}

			guid = jsonObject.settingsAssetGuid;

			string json = JsonUtility.ToJson(jsonObject, true);
			File.WriteAllText(SettingsPath, json);

			cached = settings;
		}
		
		private static AcidicGUISettings? LoadSettingsAtPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				return null;

			return AssetDatabase.LoadAssetAtPath<AcidicGUISettings>(path);
		}

		public static AcidicGUISettings? GetSettings()
		{
			if (cached != null)
				return cached;
			
			string settingsGuid = LocateSettingsAssetGuid();
			
			string assetPath = AssetDatabase.GUIDToAssetPath(settingsGuid);
			AcidicGUISettings? settingsAsset = LoadSettingsAtPath(assetPath);

			cached = settingsAsset;
			
			return settingsAsset;
		}
		
		private static string LocateSettingsAssetGuid()
		{
			// Step 1: Check if the AcidicGUI.json path exists
			if (!File.Exists(SettingsPath))
				return string.Empty;
			
			// Try to read and deserialize it.
			string json = File.ReadAllText(SettingsPath);

			try
			{
				var jsonObject = JsonUtility.FromJson<AcidicGuiJson>(json);
				return jsonObject.settingsAssetGuid;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			return string.Empty;
		}
#endif
		#endregion
	}
}