using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.WorldData.Data;
using Modules;
using OS.FileSystems;

namespace OS.Devices
{
	public sealed class PlayerHostnameFileEntry : IFileEntry
	{
		private readonly IGameContext game;
		private readonly IComputer computer;
		
		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public IDirectoryEntry Parent { get; }

		/// <inheritdoc />
		public bool CanExecute { get; } = false;

		public PlayerHostnameFileEntry(IDirectoryEntry directory, string name, IComputer computer, IGameContext game)
		{
			this.Name = name;
			this.Parent = directory;
			this.computer = computer;
			this.game = game;
		}
		
		/// <inheritdoc />
		public bool TryDelete(IUser user)
		{
			return false;
		}

		/// <inheritdoc />
		public bool TryOpenRead(IUser user, out Stream stream)
		{
			if (game.CurrentGameMode != GameMode.OnDesktop)
			{
				stream = Stream.Null;
				return false;
			}

			stream = new ReadOnlyMemoryStream((Encoding.UTF8.GetBytes(this.computer.Name)));
			return true;
		}

		/// <inheritdoc />
		public bool TryOpenWrite(IUser user, out Stream stream)
		{
			if (game.CurrentGameMode != GameMode.OnDesktop)
			{
				stream = Stream.Null;
				return false;
			}
			
			stream = new NotifyingMemoryStream(SetHostnameData);
			return true;
		}

		/// <inheritdoc />
		public bool TryOpenWriteAppend(IUser user, out Stream stream)
		{
			if (game.CurrentGameMode != GameMode.OnDesktop)
			{
				stream = Stream.Null;
				return false;
			}

			stream = new NotifyingMemoryStream(Encoding.UTF8.GetBytes(computer.Name), SetHostnameData);
			stream.Position = stream.Length;
			return true;
		}

		private void SetHostnameData(byte[] data)
		{
			string text = Encoding.UTF8.GetString(data);

			if (string.IsNullOrWhiteSpace(text))
				text = "localhost";

			game.SetPlayerHostname(text.Trim().StripNewLines());
		}
		
		/// <inheritdoc />
		public Task<bool> TryExecute(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			return Task.FromResult(false);
		}
	}
}