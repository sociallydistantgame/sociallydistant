#if UNITY_EDITOR

using GamePlatform;
using UnityEditor;

namespace Utility
{
	public static class QuickActions
	{
		private const string MenuName = "Quick Actions";

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
		
		
	}
}

#endif