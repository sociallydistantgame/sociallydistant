#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Systems;
using OS.Devices;

namespace Architecture
{
	public class SystemProcess : ISystemProcess
	{
		private readonly UniqueIntGenerator pidGenerator;
		private readonly DeviceCoordinator coordinator;

		/// <inheritdoc />
		public int Id { get; }

		/// <inheritdoc />
		public IUser User { get; }

		/// <inheritdoc />
		public ISystemProcess? Parent { get; }

		/// <inheritdoc />
		public IEnumerable<ISystemProcess> Children => coordinator.GetChildProcesses(this);

		public SystemProcess(UniqueIntGenerator pidGenerator, DeviceCoordinator coordinator, ISystemProcess parent, IUser user)
		{
			this.pidGenerator = pidGenerator;
			this.coordinator = coordinator;

			this.Id = pidGenerator.GetNextValue();
			this.User = user;
			this.Parent = parent;

			this.coordinator.DeclareProcess(this);
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