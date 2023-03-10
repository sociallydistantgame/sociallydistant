#nullable enable
using Core;
using Core.WorldData.Data;
using UnityEngine;

namespace DevTools
{
	public class CreateIspMenu : IDevMenu
	{
		private WorldManager world;
		private WorldInternetServiceProviderData isp;

		private byte octet1;
		private byte octet2;
		private byte octet3;
		private byte octet4;
		private byte cidrBits;

		public string Name => "Create ISP";
		
		public CreateIspMenu(WorldManager world)
		{
			this.world = world;
		}

		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name:");
			GUILayout.FlexibleSpace();
			this.isp.Name = GUILayout.TextField(this.isp.Name);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();

			GUILayout.Label("Subnet:");
			GUILayout.FlexibleSpace();

			octet1 = DebugGUILayout.ByteField(octet1);
			GUILayout.Label(".");
			octet2 = DebugGUILayout.ByteField(octet2);
			GUILayout.Label(".");
			octet3 = DebugGUILayout.ByteField(octet3);
			GUILayout.Label(".");
			octet4 = DebugGUILayout.ByteField(octet4);
			GUILayout.Label("/");
			cidrBits = DebugGUILayout.ByteField(cidrBits);

			if (cidrBits > 32)
				cidrBits = 32;

			GUILayout.EndHorizontal();

			if (GUILayout.Button("Create"))
			{
				isp.InstanceId = world.GetNextObjectId();
				isp.CidrNetwork = $"{octet1}.{octet2}.{octet3}.{octet4}/{cidrBits}";
				world.World.InternetProviders.Add(this.isp);
				devMenu.PopMenu();
			}
		}
	}

	public static class DebugGUILayout
	{
		public static byte ByteField(byte value)
		{
			if (byte.TryParse(GUILayout.TextField(value.ToString()), out byte newValue))
				return newValue;
			
			return value;
		}
	}
}