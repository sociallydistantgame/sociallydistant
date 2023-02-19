#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Systems;
using OS.Devices;

namespace Architecture
{
	public class LoginProcess : ISystemProcess
	{
		private DeviceCoordinator coordinator;
		private IUser loginUser;
		private readonly UniqueIntGenerator pidGenerator;
		
		/// <inheritdoc />"/>
		public int Id { get; }
		
		public LoginProcess(UniqueIntGenerator pidGenerator, DeviceCoordinator coordinator, ISystemProcess parent, IUser loginUser)
		{
			Id = pidGenerator.GetNextValue();
			this.pidGenerator = pidGenerator;
			
			this.coordinator = coordinator;
			this.loginUser = loginUser;
			this.Parent = parent;
		}
		
		/// <inheritdoc />
		public ISystemProcess? Parent { get; private set; }

		/// <inheritdoc />
		public IEnumerable<ISystemProcess> Children => coordinator.GetChildProcesses(this);

		/// <inheritdoc />
		public IUser User
		{
			get => loginUser;
			private set => loginUser = value;
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