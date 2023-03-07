#nullable enable
using System;
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

		[Header("Dependencies")]
		[SerializeField]
		private WorldManagerHolder world = null!;

		[SerializeField]
		private DeviceCoordinator deviceCoordinator = null!;
		
		/// <inheritdoc />
		public string Name => worldData.HostName;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(NonPlayerComputer));

			this.initProcess = this.deviceCoordinator.SetUpComputer(this);
		}

		/// <inheritdoc />
		public bool FindUserById(int id, out IUser? user)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public bool FindUserByName(string username, out IUser? user)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public ISystemProcess? ExecuteProgram(ISystemProcess parentProcess, ITextConsole console, string programName, string[] arguments)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public VirtualFileSystem GetFileSystem(IUser user)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public NetworkConnection? Network => null;

		public void UpdateWorldData(WorldComputerData data)
		{
			worldData = data;
		}
	}
}