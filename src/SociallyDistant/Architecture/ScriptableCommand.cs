#nullable enable
using Serilog;
using Silk.NET.SDL;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.Core.OS.Network;
using SociallyDistant.Core.OS.Tasks;

namespace SociallyDistant.Architecture
{
	public abstract class ScriptableCommand : ICommandTask
	{
		private readonly CommandContext context;
		private          string[]       arguments       = Array.Empty<string>();
		private          ISystemProcess currentProcess  = null!;
		private          ConsoleWrapper friendlyConsole = null!;

		public string HostName => currentProcess.User.Computer.Name; 
		public IUser User => Process.User;
		public IGameContext Game => context.Game;
		public string Name => Arguments.First();
		public string UserName => Process.User.UserName;
		public ConsoleWrapper Console => friendlyConsole;
		public ISystemProcess Process => currentProcess;
		public string[] Arguments => arguments;
		public IVirtualFileSystem FileSystem => Process.User.Computer.GetFileSystem(Process.User);
		public INetworkConnection? Network => Process.User.Computer.Network;
		public string CurrentWorkingDirectory => Process.WorkingDirectory;
		
		public ScriptableCommand(IGameContext gameContext)
		{
			this.context = new CommandContext(this, gameContext);
		}

		/// <inheritdoc />
		public async Task Main(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			this.currentProcess = process;
			this.arguments = arguments;
			friendlyConsole = new ConsoleWrapper(console);

			try
			{
				await OnExecute();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				await this.context.Game.Shell.ShowExceptionMessage(ex);
			}
			finally
			{
				this.currentProcess.Kill();
			}
		}

		protected abstract Task OnExecute();
		
		public sealed class CommandContext
		{
			private readonly ScriptableCommand command;
			private readonly IGameContext          gameContext;

			public IGameContext Game => gameContext;
			
			internal CommandContext(ScriptableCommand command, IGameContext gameContext)
			{
				this.gameContext = gameContext;
				this.command = command;
			}
		}
	}
}