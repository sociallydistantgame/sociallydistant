#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Systems;
using OS.Devices;

namespace Architecture
{
	public class DeviceCoordinatorProcess : IInitProcess
	{
		private readonly DeviceCoordinator coordinator;
		private readonly UniqueIntGenerator pidGenerator = new UniqueIntGenerator();

		/// <inheritdoc />
		public int Id { get; }

		/// <inheritdoc />"/>
		public string Name { get; set; }
		
		/// <inheritdoc />
		public IUser User { get; private set; }

		/// <inheritdoc />
		public ISystemProcess? Parent => null;

		/// <inheritdoc />
		public IEnumerable<ISystemProcess> Children => coordinator.GetChildProcesses(this);

		public DeviceCoordinatorProcess(DeviceCoordinator coordinator, IComputer computer)
		{
			this.Id = pidGenerator.GetNextValue();
			Name = "init";
			this.coordinator = coordinator;
			
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
			return new SystemProcess(
				pidGenerator,
				coordinator,
				this,
				User
			);
		}
		
		/// <inheritdoc />
		public ISystemProcess ForkAsUser(IUser user)
		{
			if (user == User)
				return Fork();
			
			// Prevent users not from the same computer from
			// executing processes on it.
			if (user.Computer != User.Computer)
				throw new InvalidOperationException("An invalid attempt was made to execute a process on one computer by the user of another computer.");

			IUser previousUser = this.User;
			this.User = user;
			ISystemProcess forked = Fork();

			this.User = previousUser;
			return forked;
		}

		/// <inheritdoc />
		public ISystemProcess CreateLoginProcess(IUser user)
		{
			var proc = new LoginProcess(pidGenerator, coordinator, this, user);
			coordinator.DeclareProcess(proc);
			return proc;
		}
		
		/// <inheritdoc />
		public event Action<ISystemProcess>? Killed; 

		/// <inheritdoc />
		public void Kill()
		{
			foreach (ISystemProcess child in Children.ToArray())
				child.Kill();
			
			Killed?.Invoke(this);
		}
	}
}