#if UNITY_EDITOR

using Core.Config;
using GamePlatform;
using UnityEditor;
using UnityEngine.Device;

namespace Utility
{
	public static class QuickActions
	{
		private const string MenuName = "Quick Actions";

		[MenuItem(itemName: MenuName + "/Open Game Data")]
		private static void OpenGameData()
		{
			System.Diagnostics.Process.Start(Application.persistentDataPath);
		}
		
		[MenuItem(itemName: MenuName + "/Initialization Flow/Debug World")]
		public static void InitFlow_DebugWorld()
		{
			EditorPrefs.SetInt(GameManager.InitializationFlowEditorPreference, (int) GameManager.InitializationFlow.DebugWorld);
		}
		
		[MenuItem(itemName: MenuName + "/Initialization Flow/Load Most Recent Save")]
		public static void InitFlow_MostRecentSave()
		{
			EditorPrefs.SetInt(GameManager.InitializationFlowEditorPreference, (int) GameManager.InitializationFlow.MostRecentSave);
		}
		
		[MenuItem(itemName: MenuName + "/Initialization Flow/Login Screen")]
		public static void InitFlow_LoginScreen()
		{
			EditorPrefs.SetInt(GameManager.InitializationFlowEditorPreference, (int) GameManager.InitializationFlow.LoginScreen);
		}
		
		[MenuItem(MenuName + "/Accept legal agreement for script mods")]
		public static void AcceptScriptModLegalAgreement()
		{
			using var settingsManager = new SettingsManager();

			var moddingSettings = new ModdingSettings(settingsManager);

			if (moddingSettings.AcceptLegalWaiver)
			{
				EditorUtility.DisplayDialog("Script mods already enabled!", "You have already accepted the legal agreement for script mod execution.", "OK");
				return;
			}

			moddingSettings.AcceptLegalWaiver = EditorUtility.DisplayDialog(
				"Enable script mods?",
				@"Script mods can add additional functionality to Socially Distant by injecting their code directly into the game's process. Script mods are not sandboxed in any way, and can run arbitrary code on your device.+
				
By enabling script mod execution, you acknowledge the risk of executing malicious code on your device, and you agree that Socially Distant's development team is neither responsible nor liable for any damages caused by installed script mods.
				
With that in mind, do you want to enable script mods?",
				"Yes",
				"No");

			settingsManager.Save();
		}
	}
}

#endif