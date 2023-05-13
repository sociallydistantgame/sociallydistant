#nullable enable
using System;
using Core;
using Core.DataManagement;
using Core.WorldData.Data;
using GameplaySystems.Networld;
using UnityEngine;

namespace DevTools
{
	public class CreateHackableMenu : IDevMenu
	{
		private WorldManager world; 
		private ushort port;
		private ObjectId computerId;
		private ServerType serverType;
		private SecurityLevel secLevel;
		private bool selectingServerType;
		private bool selectingComputer;
		private string[] serverTypeNames;
		private WorldComputerData[] computers;

		public CreateHackableMenu(WorldManager world)
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

			GUILayout.BeginHorizontal();
			GUILayout.Label("Computer: ");
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Select"))
			{
				computers = world.World.Computers.ToArray();
				selectingComputer = true;
			}

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Port");
			GUILayout.FlexibleSpace();
			port = DebugGUILayout.UShortField(port);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Server Type:");
			GUILayout.FlexibleSpace();
			GUILayout.Label(serverType.ToString());
			if (GUILayout.Button("Change"))
				selectingServerType = true;
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Security Level:");
			GUILayout.FlexibleSpace();
			GUILayout.Label(secLevel.ToString());

			if (GUILayout.Button("Open"))
				secLevel = SecurityLevel.Open;
			if (GUILayout.Button("Secure"))
				secLevel = SecurityLevel.Secure;
			if (GUILayout.Button("Hardened"))
				secLevel = SecurityLevel.Hardened;
			if (GUILayout.Button("Unhackable"))
				secLevel = SecurityLevel.Unhackable;
			
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Create"))
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
				if (GUILayout.Button(computers[i].HostName))
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
				if (GUILayout.Button(serverTypeNames[i]))
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