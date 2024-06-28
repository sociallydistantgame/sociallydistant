#nullable enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools
{
	public class SetComputerLANMenu : IDevMenu
	{
		private WorldManager world;
		private WorldComputerData[] computers;
		private WorldLocalNetworkData[] lans;
		private int lanIndex = -1;
		private int computerIndex = -1;

		/// <inheritdoc />
		public string Name => "Set Computer LAN";

		public SetComputerLANMenu(WorldManager world)
		{
			this.world = world;
			this.computers = this.world.World.Computers.ToArray();
			this.lans = this.world.World.LocalAreaNetworks.ToArray();
			
		}

		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			if (computerIndex == -1)
			{
				for (var i = 0; i < computers.Length; i++)
				{
					if (!ImGui.Button(computers[i].HostName))
						continue;

					computerIndex = i;
					break;
				}

				return;
			}

			if (lanIndex == -1)
			{
				for (var i = 0; i < lans.Length; i++)
				{
					if (!ImGui.Button(lans[i].Name))
						continue;

					lanIndex = i;
					break;
				}
				
				return;
			}

			ObjectId lanId = lans[lanIndex].InstanceId;
			ObjectId computerId = computers[computerIndex].InstanceId;

			WorldNetworkConnection connection = world.World.NetworkConnections.ToArray().FirstOrDefault(x => x.ComputerId == computerId);
			if (connection.ComputerId == computerId)
			{
				connection.LanId = lanId;
				world.World.NetworkConnections.Modify(connection);
			}
			else
			{
				connection.InstanceId = world.GetNextObjectId();
				connection.ComputerId = computerId;
				connection.LanId = lanId;
				world.World.NetworkConnections.Add(connection);
			}
			
			devMenu.PopMenu();
		}
	}
}