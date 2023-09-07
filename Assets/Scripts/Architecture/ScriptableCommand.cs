#nullable enable
using GameplaySystems.Networld;
using OS.Devices;
using OS.FileSystems;
using OS.Network;
using UI.Terminal;
using UnityEngine;

namespace Architecture
{
	public abstract class ScriptableCommand : ScriptableCommandBase
	{
		[SerializeField]
		private bool autoKillOnComplete = true;

		private ISystemProcess process = null!;
		
		protected ConsoleWrapper Console { get; private set; } = null!;
		protected string[] Arguments { get; private set; } = null!;
		protected IVirtualFileSystem FileSystem { get; private set; } = null!;
		protected INetworkConnection? Network => this.process.User.Computer.Network;
		protected string UserName => process.User.UserName;
		protected string HostName => process.User.Computer.Name;

		protected string CurrentWorkingDirectory => process.WorkingDirectory;
		
		/// <inheritdoc />
		public override void Main(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			this.process = process;
			
			Console = new ConsoleWrapper(console);
			Arguments = arguments;
			FileSystem = process.User.Computer.GetFileSystem(process.User);
			
			OnExecute();

			if (autoKillOnComplete)
				process.Kill();
		}

		protected abstract void OnExecute();

		protected void EndProcess()
		{
			process.Kill();
		}
	}
}