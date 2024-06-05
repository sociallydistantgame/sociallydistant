#nullable enable
using System;
using System.Collections.Generic;
using Architecture;
using Core;
using Core.WorldData.Data;
using GamePlatform;
using GameplaySystems.Networld;
using OS.Devices;
using OS.FileSystems;
using OS.Network;
using UnityEngine;
using UnityExtensions;
using Utility;
using System.Threading.Tasks;

namespace GameplaySystems.NonPlayerComputers
{
	public class NonPlayerComputer :
		MonoBehaviour,
		IComputer
	{
		[SerializeField]
		private DeviceCoordinator deviceCoordinator = null!;
		
		[Header("Computer configuration")]
		[SerializeField]
		private EnvironmentVariablesAsset environmentVariables = null!;

		[SerializeField]
		private FileSystemTableAsset fileSystemTable = null!;
		
		private WorldComputerData worldData;
		private ISystemProcess initProcess = null!;
		private LocalAreaNetwork? currentLan;
		private NonPlayerNetworkConnection networkConnection;
		private SuperUser su;
		private NonPlayerFileSystem fs;
		private NpcFileOverrider fileOverrider;
		private ISystemProcess systemd;
		private IWorldManager world = null!;
		private ServiceManager? serviceManager;
		
		/// <inheritdoc />
		public string Name => worldData.HostName;

		private async void Awake()
		{
			networkConnection = new NonPlayerNetworkConnection(this);
			
			world = GameManager.Instance.WorldManager;
			
			this.AssertAllFieldsAreSerialized(typeof(NonPlayerComputer));

			this.su = new SuperUser(this);
			RebuildVfs();
			
			this.initProcess = this.deviceCoordinator.SetUpComputer(this, null);
			this.initProcess.Environment["PS1"] = "[%u@%h %W]%$ ";
			this.systemd = await this.initProcess.Fork();
			this.systemd.Name = "systemd";
			
			// Apply environment variables to the system
			foreach (KeyValuePair<string, string> keyPair in environmentVariables)
				initProcess.Environment[keyPair.Key] = keyPair.Value;

			serviceManager = new ServiceManager(initProcess);
			serviceManager.UpdateServices(worldData.Services);
		}

		private void Update()
		{
			serviceManager?.Update();
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
		public IUser SuperUser => su;

		/// <inheritdoc />
		public async Task<ISystemProcess?> ExecuteProgram(ISystemProcess parentProcess, ITextConsole console, string programName, string[] arguments)
		{
			// Perhaps we should remove this in favour of calling the VFS method directly?
			return await GetFileSystem(parentProcess.User)
				.Execute(parentProcess, programName, console, arguments);
		}

		/// <inheritdoc />
		public IVirtualFileSystem GetFileSystem(IUser user)
		{
			return new VirtualFileSystem(this.fs, user, this.fileOverrider);
		}

		/// <inheritdoc />
		public INetworkConnection? Network => networkConnection;

		/// <inheritdoc />
		public async Task<ISystemProcess?> CreateDaemonProcess(string name)
		{
			ISystemProcess? result = await systemd?.Fork();
			if (result != null)
				result.Name = name;
			return result;
		}

		public void UpdateWorldData(WorldComputerData data)
		{
			worldData = data;
			RebuildVfs();

			this.serviceManager?.UpdateServices(data.Services);
		}

		public void ConnectLan(LocalAreaNetwork lan)
		{
			serviceManager?.UpdateServices(Array.Empty<NetworkServiceData>());
			networkConnection.Connect(lan);
			serviceManager?.UpdateServices(worldData.Services);
		}
		
		public void DisconnectLan()
		{
			serviceManager?.UpdateServices(Array.Empty<NetworkServiceData>());
			networkConnection.Disconnect();
			serviceManager?.UpdateServices(worldData.Services);
		}

		private void RebuildVfs()
		{
			if (!this.gameObject.activeSelf)
				return;

			this.fs = new NonPlayerFileSystem(this, worldData.InstanceId, this.world);
			
			// Mount file systems from Unity
			FileSystemTable.MountFileSystemsToComputer(this, this.fileSystemTable);
		}

		public void SetFileOverrider(NpcFileOverrider overrider)
		{
			this.fileOverrider = overrider;
		}
	}
}