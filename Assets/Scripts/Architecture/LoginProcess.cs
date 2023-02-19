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
		public IUser User => loginUser;

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