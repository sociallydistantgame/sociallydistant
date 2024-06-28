#nullable enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools
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
			ImGui.Text("Name:");
			//this.isp.Name = GUILayout.TextField(this.isp.Name);
			
			ImGui.Text("Subnet:");
			
			octet1 = DebugGUILayout.ByteField(octet1);
			ImGui.Text(".");
			octet2 = DebugGUILayout.ByteField(octet2);
			ImGui.Text(".");
			octet3 = DebugGUILayout.ByteField(octet3);
			ImGui.Text(".");
			octet4 = DebugGUILayout.ByteField(octet4);
			ImGui.Text("/");
			cidrBits = DebugGUILayout.ByteField(cidrBits);

			if (cidrBits > 32)
				cidrBits = 32;

			if (ImGui.Button("Create"))
			{
				isp.InstanceId = world.GetNextObjectId();
				isp.CidrNetwork = $"{octet1}.{octet2}.{octet3}.{octet4}/{cidrBits}";
				world.World.InternetProviders.Add(this.isp);
				devMenu.PopMenu();
			}
		}
	}
}