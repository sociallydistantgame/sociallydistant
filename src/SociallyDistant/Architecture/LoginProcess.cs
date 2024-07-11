#nullable enable
using System.Runtime.CompilerServices;
using SociallyDistant.Core.Core.Systems;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Architecture
{
	public class LoginProcess : ISystemProcess
	{
		private readonly UniqueIntGenerator        pidGenerator;
		private readonly TaskCompletionSource<int> exitCodeSource = new();
		private          DeviceCoordinator         coordinator;
		private          IUser                     loginUser;
		private          bool                      isAlive = true;
		private          int                       exitCode;

		/// <inheritdoc />
		public int ExitCode => exitCode;
		
		/// <inheritdoc />"/>
		public string Name { get; set; }

		/// <inheritdoc />"/>
		public bool IsAlive => isAlive;
		
		/// <inheritdoc />
		public IEnvironmentVariableProvider Environment { get; }
		
		/// <inheritdoc />"/>
		public int Id { get; }
		
		public string WorkingDirectory { get; set; }
		
		internal LoginProcess(UniqueIntGenerator pidGenerator, DeviceCoordinator coordinator, ISystemProcess parent, IUser loginUser)
		{
			Id = pidGenerator.GetNextValue();
			Name = "syslogin";
			this.pidGenerator = pidGenerator;
			this.WorkingDirectory = parent.WorkingDirectory;
			this.Environment = parent.Environment.DeepClone();
			
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
		public void Kill(int exitCode = 0)
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