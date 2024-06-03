#nullable enable

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GameplaySystems.Networld;
using OS.Devices;
using OS.Tasks;
using OS.FileSystems;
using OS.Network;
using UI.Terminal;
using UnityEngine;

namespace Architecture
{
	public abstract class CommandScript :
		MonoBehaviour,
		ICommandTask
	{
		private bool mainGuard;

		private string[] args;
		private ConsoleWrapper console;
		private ISystemProcess proc;

		protected string[] Arguments => args;
		protected ConsoleWrapper Console => console;
		protected INetworkConnection? Network => proc.User.Computer.Network;
		protected IVirtualFileSystem FileSystem { get; private set; }
		protected string WorkingDirectory => proc.WorkingDirectory;
		protected string HomeDirectory => proc.User.Home;
		protected IUser User => proc.User;

		public string ProcessName => proc.Name;
		
		/// <inheritdoc />
		public async Task Main(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			// Prevents stupid people from running this
			if (mainGuard)
				throw new InvalidOperationException("Command script has already been initialized.");

			this.mainGuard = true;
			this.proc = process;
			this.console = new ConsoleWrapper(console);
			this.args = arguments;

			FileSystem = proc.User.Computer.GetFileSystem(proc.User);

			proc.Killed += HandleProcessKilled;

			await OnMain();
		}

		private void HandleProcessKilled(ISystemProcess obj)
		{
			proc.Killed -= HandleProcessKilled;
			proc = null!;
			Destroy(this.gameObject);
		}

		private void OnDestroy()
		{
			proc?.Kill();
		}

		protected virtual Task OnMain()
		{
			return Task.CompletedTask;
		}

		protected void EndProcess()
		{
			proc.Kill();
		}

		protected bool TryGetEnvironmentVariable(string variable, out string? value)
		{
			value = default;
			if (this.proc == null!)
				return false;

			if (proc.Environment.IsSet(variable))
			{
				value = proc.Environment[variable];
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}