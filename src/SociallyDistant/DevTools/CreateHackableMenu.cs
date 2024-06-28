#nullable enable
using ImGuiNET;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.OS.Network;

namespace SociallyDistant.DevTools
{
	public class CreateHackableMenu : IDevMenu
	{
		private IWorldManager world; 
		private ushort port;
		private ObjectId computerId;
		private ServerType serverType;
		private SecurityLevel secLevel;
		private bool selectingServerType;
		private bool selectingComputer;
		private string[] serverTypeNames;
		private WorldComputerData[] computers;

		public CreateHackableMenu(IWorldManager world)
		{
			this.world = world;
			serverTypeNames = Enum.GetNames(typeof(ServerType));
		}

		/// <inheritdoc />
		public string Name => "Create Hackable";

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (HandleComputerSelect(ref selectingComputer))
				return;
			
			if (HandleServerType(ref selectingServerType))
				return;

			ImGui.Text("Computer: ");
			if (ImGui.Button("Select"))
			{
				computers = world.World.Computers.ToArray();
				selectingComputer = true;
			}
			
			ImGui.Text("Port");
			port = DebugGUILayout.UShortField(port);

			ImGui.Text("Server Type:");
			ImGui.Text(serverType.ToString());
			if (ImGui.Button("Change"))
				selectingServerType = true;

			ImGui.Text("Security Level:");
			ImGui.Text(secLevel.ToString());

			if (ImGui.Button("Open"))
				secLevel = SecurityLevel.Open;
			if (ImGui.Button("Secure"))
				secLevel = SecurityLevel.Secure;
			if (ImGui.Button("Hardened"))
				secLevel = SecurityLevel.Hardened;
			if (ImGui.Button("Unhackable"))
				secLevel = SecurityLevel.Unhackable;

			if (ImGui.Button("Create"))
			{
				world.World.Hackables.Add(new WorldHackableData
				{
					InstanceId = world.GetNextObjectId(),
					ComputerId = computerId,
					Port = port,
					ServerType = serverType,
					SecurityLevel = secLevel
				});
				
				devMenu.PopMenu();
			}
		}

		private bool HandleComputerSelect(ref bool selecting)
		{
			if (!selecting)
				return false;

			for (var i = 0; i < computers.Length; i++)
			{
				if (ImGui.Button(computers[i].HostName))
				{
					selecting = false;
					computerId = computers[i].InstanceId;
					return false;
				}
			}
			
			return true;
		}
		
		private bool HandleServerType(ref bool selecting)
		{
			if (!selecting)
				return false;

			for (var i = 0; i < serverTypeNames.Length; i++)
			{
				if (ImGui.Button(serverTypeNames[i]))
				{
					serverType = (ServerType) i;
					selecting = false;
					return false;
				}
			}

			return true;
		}
	}
}