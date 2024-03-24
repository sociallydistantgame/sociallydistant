#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Systems;
using GamePlatform;
using OS.Devices;
using OS.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Architecture
{
	public class DeviceCoordinatorProcess : IInitProcess
	{
		private readonly Dictionary<IUser, ISystemProcess> userProcesses = new Dictionary<IUser, ISystemProcess>();
		private readonly DeviceCoordinator coordinator;
		private readonly UniqueIntGenerator pidGenerator = new UniqueIntGenerator();
		private readonly SimpleEnvironmentVariableProvider environment = new SimpleEnvironmentVariableProvider();
		private readonly IShellScript loginScript;
		private bool isAlive = true;
		private int exitCode;

		/// <inheritdoc />
		public bool IsAlive => isAlive;

		/// <inheritdoc />
		public int ExitCode => exitCode;
        
		/// <inheritdoc />
		public string WorkingDirectory { get; set; } = "/";
		
		/// <inheritdoc />
		public int Id { get; }

		/// <inheritdoc />"/>
		public string Name { get; set; }
		
		/// <inheritdoc />
		public IUser User { get; private set; }

		/// <inheritdoc />
		public IEnvironmentVariableProvider Environment => this.environment;
		
		/// <inheritdoc />
		public ISystemProcess? Parent => null;

		/// <inheritdoc />
		public IEnumerable<ISystemProcess> Children => coordinator.GetChildProcesses(this);

		public DeviceCoordinatorProcess(DeviceCoordinator coordinator, IComputer computer, IShellScript loginScript)
		{
			this.Id = pidGenerator.GetNextValue();
			Name = "init";
			this.coordinator = coordinator;
			this.loginScript = loginScript;
			
			// Find a root user, throw if we can't.
			if (!computer.FindUserById(0, out IUser? root) || root == null)
				throw new InvalidOperationException("Cannot find root user with user ID 0");

			if (root.PrivilegeLevel != PrivilegeLevel.Root)
				throw new InvalidOperationException("Only users with Root privilege level may be used to set up the root process of a computer.");
			
			this.User = root;
		}
		
		/// <inheritdoc />
		public ISystemProcess Fork()
		{
			return ForkAsUser(this.User);
		}
		
		/// <inheritdoc />
		public ISystemProcess ForkAsUser(IUser user)
		{
			// Prevent users not from the same computer from
			// executing processes on it.
			if (user.Computer != User.Computer)
				throw new InvalidOperationException("An invalid attempt was made to execute a process on one computer by the user of another computer.");


			return CreateLoginProcess(user).Fork();
		}

		/// <inheritdoc />
		public ISystemProcess CreateLoginProcess(IUser user)
		{
			if (!userProcesses.TryGetValue(user, out ISystemProcess userProcess))
			{
				userProcess = new LoginProcess(pidGenerator, coordinator, this, user);

				if (loginScript == null)
				{
					Debug.LogWarning("loginScript is null for this device, users won't get any default settings.");
				}
				else
				{
					loginScript.Run(userProcess, Array.Empty<string>(), new UnityTextConsole())
						.GetAwaiter()
						.GetResult();
				}

				this.userProcesses.Add(user, userProcess);			
				
				coordinator.DeclareProcess(userProcess);
			}
			
			return userProcess;
		}
		
		/// <inheritdoc />
		public event Action<ISystemProcess>? Killed; 

		/// <inheritdoc />
		public void Kill(int exitCode = 0)
		{
			foreach (ISystemProcess child in Children.ToArray())
				child.Kill();

			this.exitCode = exitCode;
			isAlive = false;
			Killed?.Invoke(this);
		}
	}
}