#nullable enable

using System;
using System.Diagnostics;
using GameplaySystems.Networld;
using OS.Devices;
using OS.Tasks;
using OS.FileSystems;
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
		protected NetworkConnection? Network => proc.User.Computer.Network;
		protected VirtualFileSystem FileSystem { get; private set; }
		protected string WorkingDirectory => proc.WorkingDirectory;
		protected string HomeDirectory => proc.User.Home;

		public string ProcessName => proc.Name;
		
		/// <inheritdoc />
		public void Main(ISystemProcess process, ITextConsole console, string[] arguments)
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

			OnMain();
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

		protected virtual void OnMain()
		{
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