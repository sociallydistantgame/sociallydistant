#nullable enable
using System;
using System.Collections.Generic;
using Architecture;
using Core;
using Core.WorldData.Data;
using GameplaySystems.Networld;
using OS.Devices;
using OS.FileSystems;
using OS.Network;
using UnityEngine;
using Utility;

namespace GameplaySystems.NonPlayerComputers
{
	public class NonPlayerComputer :
		MonoBehaviour,
		IComputer
	{
		private WorldComputerData worldData;
		private ISystemProcess initProcess = null!;
		private LocalAreaNetwork? currentLan;
		private NetworkConnection? networkConnection;
		private SuperUser su;
		private NonPlayerFileSystem fs;
		private NpcFileOverrider fileOverrider;

		[Header("Dependencies")]
		[SerializeField]
		private WorldManagerHolder world = null!;

		[SerializeField]
		private DeviceCoordinator deviceCoordinator = null!;

		[SerializeField]
		private NetworkSimulationHolder networkSimulation = null!;

		[Header("Computer configuration")]
		[SerializeField]
		private EnvironmentVariablesAsset environmentVariables = null!;

		[SerializeField]
		private FileSystemTableAsset fileSystemTable = null!;
		
		/// <inheritdoc />
		public string Name => worldData.HostName;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(NonPlayerComputer));

			this.su = new SuperUser(this);
			RebuildVfs();
			
			this.initProcess = this.deviceCoordinator.SetUpComputer(this);
			
			// Apply environment variables to the system
			foreach (KeyValuePair<string, string> keyPair in environmentVariables)
				initProcess.Environment[keyPair.Key] = keyPair.Value;
		}

		/// <inheritdoc />
		public bool FindUserById(int id, out IUser? user)
		{
			if (id == su.Id)
			{
				user = su;
				return true;
			}

			// TODO: Non-root users.
			user = null;
			return false;
		}

		/// <inheritdoc />
		public bool FindUserByName(string username, out IUser? user)
		{
			if (username == su.UserName)
			{
				user = su;
				return true;
			}

			user = null;
			return false;
		}

		/// <inheritdoc />
		public ISystemProcess? ExecuteProgram(ISystemProcess parentProcess, ITextConsole console, string programName, string[] arguments)
		{
			// Perhaps we should remove this in favour of calling the VFS method directly?
			return GetFileSystem(parentProcess.User)
				.Execute(parentProcess, programName, console, arguments);
		}

		/// <inheritdoc />
		public VirtualFileSystem GetFileSystem(IUser user)
		{
			return new VirtualFileSystem(this.fs, user, this.fileOverrider);
		}

		/// <inheritdoc />
		public NetworkConnection? Network => networkConnection;

		public void UpdateWorldData(WorldComputerData data)
		{
			worldData = data;
			RebuildVfs();
		}

		public void ConnectLan(LocalAreaNetwork lan)
		{
			if (currentLan == lan)
				return;

			currentLan = lan;
			networkConnection = currentLan.CreateDevice();
		}
		
		public void DisconnectLan()
		{
			if (currentLan == null)
				return;

			if (this.networkConnection!=null)
				currentLan.DeleteDevice(this.networkConnection);

			this.networkConnection = null;
			this.currentLan = null;
		}

		private void RebuildVfs()
		{
			if (!this.gameObject.activeSelf)
				return;

			this.fs = new NonPlayerFileSystem(this, worldData.InstanceId, this.world.Value);
			
			// Mount file systems from Unity
			FileSystemTable.MountFileSystemsToComputer(this, this.fileSystemTable);
		}

		public void SetFileOverrider(NpcFileOverrider overrider)
		{
			this.fileOverrider = overrider;
		}
	}
}