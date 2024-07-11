#nullable enable
using System.Runtime.CompilerServices;
using SociallyDistant.Core.Core.Systems;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Architecture
{
	public class SystemProcess : ISystemProcess
	{
		private readonly UniqueIntGenerator        pidGenerator;
		private readonly DeviceCoordinator         coordinator;
		private readonly TaskCompletionSource<int> exitCodeSource = new();
		private          bool                      isAlive        = true;
		private          int                       exitCode;

		/// <inheritdoc />
		public int ExitCode => exitCode;
		
		/// <inheritdoc />
		public bool IsAlive => isAlive;
		
		/// <inheritdoc />
		public int Id { get; }

		/// <inheritdoc />"/>
		public string Name { get; set; }
		
		/// <inheritdoc />
		public IUser User { get; private set; }

		/// <inheritdoc />
		public ISystemProcess? Parent { get; }

		/// <inheritdoc />
		public IEnumerable<ISystemProcess> Children => coordinator.GetChildProcesses(this);

		/// <inheritdoc />
		public string WorkingDirectory { get; set; }
		
		/// <inheritdoc />
		public IEnvironmentVariableProvider Environment { get; }
		
		internal SystemProcess(UniqueIntGenerator pidGenerator, DeviceCoordinator coordinator, ISystemProcess parent, IUser user)
		{
			this.pidGenerator = pidGenerator;
			this.coordinator = coordinator;
			this.WorkingDirectory = parent.WorkingDirectory;
			this.Environment = parent.Environment.DeepClone();
			this.Id = pidGenerator.GetNextValue();
			this.Name = parent.Name;
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

			if (this.User != user)
			{
				this.coordinator.CopyEnvironment(user, forked);
			}
			
			return forked;
		}
		
		/// <inheritdoc />
		public event Action<ISystemProcess>? Killed; 

		/// <inheritdoc />
		public void Kill(int exitCode)
		{
			foreach (ISystemProcess child in Children.ToArray())
				child.Kill();

			this.exitCode = exitCode;
			isAlive = false;
			Killed?.Invoke(this);
			exitCodeSource.SetResult(exitCode);
		}

		public TaskAwaiter<int> GetAwaiter()
		{
			return exitCodeSource.Task.GetAwaiter();
		}
	}
}