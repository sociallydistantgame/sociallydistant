#nullable enable
using SociallyDistant.Architecture;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.Core.OS.Network;
using SociallyDistant.GameplaySystems.Networld;
using SociallyDistant.OS.Devices;
using SociallyDistant.OS.FileSystems;

namespace SociallyDistant.GameplaySystems.NonPlayerComputers
{
	public class NonPlayerComputer : IComputer
	{
		private readonly SociallyDistantGame        game;
		private readonly NonPlayerNetworkConnection networkConnection;
		private readonly SuperUser                  su;
		private readonly ISystemProcess             initProcess          = null!;
		private readonly ISystemProcess             systemd;
		private readonly ServiceManager             serviceManager;


		
		private WorldComputerData worldData;
		private LocalAreaNetwork? currentLan;
		private NonPlayerFileSystem fs;
		private NpcFileOverrider fileOverrider;

		private DeviceCoordinator DeviceCoordinator => game.DeviceCoordinator;
		private IWorldManager World => game.WorldManager;
		
		public bool IsPlayer => false;

		/// <inheritdoc />
		public string Name => worldData.HostName;

		internal NonPlayerComputer(SociallyDistantGame game)
		{
			this.game = game;
			this.networkConnection = new NonPlayerNetworkConnection(this);
			this.su = new SuperUser(this);
			this.initProcess = this.DeviceCoordinator.SetUpComputer(this);
			this.initProcess.Environment["PS1"] = "[%u@%h %W]%$ ";
			this.initProcess.Environment["PATH"] = "/bin:/sbin:/usr/bin:/usr/sbin";
			this.systemd = this.initProcess.Fork();
			this.systemd.Name = "systemd";
			this.serviceManager = new ServiceManager(initProcess);
		}

		public void Update()
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
			ISystemProcess? result = systemd?.Fork();
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
			this.fs = new NonPlayerFileSystem(this, worldData.InstanceId, this.World);
		}

		public void SetFileOverrider(NpcFileOverrider overrider)
		{
			this.fileOverrider = overrider;
		}
	}
}