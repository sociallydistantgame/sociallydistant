#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using OS.Devices;
using OS.Tasks;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu(menuName = "ScriptableObject/Architecture/Device Coordinator")]
	public class DeviceCoordinator :
		ScriptableObject,
		ITaskManager
	{
		private readonly Dictionary<IComputer, IInitProcess> computers = new Dictionary<IComputer, IInitProcess>();
		private readonly List<ISystemProcess> processes = new List<ISystemProcess>();

		/// <inheritdoc />
		public IEnumerable<ISystemProcess> GetTasksForUser(IUser user)
		{
			return processes.Where(x => x.User == user);
		}

		/// <inheritdoc />
		public IEnumerable<ISystemProcess> GetTasksOnComputer(IComputer computer)
		{
			return processes.Where(x => x.User.Computer == computer);
		}

		/// <inheritdoc />
		public IEnumerable<ISystemProcess> GetChildProcesses(ISystemProcess parent)
		{
			return processes.Where(x => x.Parent == parent);
		}

		/// <inheritdoc />
		public IInitProcess SetUpComputer(IComputer computer)
		{
			// Throw if the computer is already set up.
			if (computers.ContainsKey(computer))
				throw new InvalidOperationException("The specified computer has already been registered with the device coordinator.");

			var initProcess = new DeviceCoordinatorProcess(this, computer);
			this.computers.Add(computer, initProcess);

			DeclareProcess(initProcess);
			return initProcess;
		}
		
		public void ForgetComputer(IComputer computer)
		{
			if (!computers.ContainsKey(computer))
				return;
			
			computers.Remove(computer);
		}

		public void DeclareProcess(ISystemProcess process)
		{
			if (processes.Contains(process))
				return;
			
			processes.Add(process);
			process.Killed += HandleProcessKilled;
		}

		private void HandleProcessKilled(ISystemProcess process)
		{
			processes.Remove(process);
			process.Killed -= HandleProcessKilled;

			Debug.Log($"Process {process.Id} on {process.User.Computer.Name} ran by user {process.User.UserName}: Killed");
		}
	}
}