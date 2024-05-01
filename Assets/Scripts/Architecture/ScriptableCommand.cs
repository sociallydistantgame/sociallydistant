#nullable enable
using System.Threading.Tasks;
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
		protected ISystemProcess Process => process;
		
		protected string CurrentWorkingDirectory => process.WorkingDirectory;
		
		/// <inheritdoc />
		public override async Task Main(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			this.process = process;
			
			Console = new ConsoleWrapper(console);
			Arguments = arguments;
			FileSystem = process.User.Computer.GetFileSystem(process.User);
			
			await OnExecute();

			if (autoKillOnComplete)
				process.Kill();
		}

		protected abstract Task OnExecute();

		protected void EndProcess()
		{
			process.Kill();
		}
	}
}