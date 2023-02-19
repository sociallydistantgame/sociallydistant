#nullable enable
using System.Collections.Generic;
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
	}
}