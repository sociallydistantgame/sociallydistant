#nullable enable
using ImGuiNET;
using SociallyDistant.Core;
using SociallyDistant.Core.Core.WorldData.Data;

namespace SociallyDistant.DevTools
{
	public sealed class CreateComputerMenu : IDevMenu
	{
		private WorldManager world;
		private WorldComputerData computer;


		/// <inheritdoc />
		public string Name => "Create Computer";

		public CreateComputerMenu(WorldManager world)
		{
			this.world = world;
		}
		
		/// <inheritdoc />
		public void OnMenuGUI(DeveloperMenu devMenu)
		{
			ImGui.Columns(2);
			ImGui.Text("Name:");
			ImGui.NextColumn();
			computer.HostName = computer.HostName;
			ImGui.Columns(1);

			if (ImGui.Button("Create"))
			{
				computer.InstanceId = world.GetNextObjectId();
				//computer.MacAddress = (long) Random.Range(0, long.MaxValue >> 16);
				
				world.World.Computers.Add(computer);
				devMenu.PopMenu();
			}
		}
	}
}