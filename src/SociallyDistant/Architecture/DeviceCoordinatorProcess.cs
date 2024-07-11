#nullable enable
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Serilog;
using SociallyDistant.Core.Core.Systems;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.Tasks;
using SociallyDistant.GamePlatform;

namespace SociallyDistant.Architecture
{
	public class DeviceCoordinatorProcess : IInitProcess
	{
		private readonly Dictionary<IUser, ISystemProcess> userProcesses = new Dictionary<IUser, ISystemProcess>();
		private readonly DeviceCoordinator                 coordinator;
		private readonly UniqueIntGenerator                pidGenerator   = new UniqueIntGenerator();
		private readonly SimpleEnvironmentVariableProvider environment    = new SimpleEnvironmentVariableProvider();
		private readonly TaskCompletionSource<int>         exitCodeSource = new();
		private          bool                              isAlive        = true;
		private          int                               exitCode;

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

		internal DeviceCoordinatorProcess(DeviceCoordinator coordinator, IComputer computer)
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
			return ForkAsUser(this.User);
		}
		
		/// <inheritdoc />
		public ISystemProcess ForkAsUser(IUser user)
		{
			// Prevent users not from the same computer from
			// executing processes on it.
			if (user.Computer != User.Computer)
				throw new InvalidOperationException("An invalid attempt was made to execute a process on one computer by the user of another computer.");


			ISystemProcess loginProcess = CreateLoginProcess(user);

			return loginProcess.Fork();
		}

		/// <inheritdoc />
		public ISystemProcess CreateLoginProcess(IUser user)
		{
			if (!userProcesses.TryGetValue(user, out ISystemProcess userProcess))
			{
				userProcess = new LoginProcess(pidGenerator, coordinator, this, user);

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
			exitCodeSource.SetResult(exitCode);
		}

		public TaskAwaiter<int> GetAwaiter()
		{
			return exitCodeSource.Task.GetAwaiter();
		}
	}
}