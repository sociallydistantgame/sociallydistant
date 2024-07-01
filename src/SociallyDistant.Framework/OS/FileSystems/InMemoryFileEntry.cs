#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.OS.FileSystems
{
	public sealed class InMemoryFileEntry : IFileEntry
	{
		private readonly InMemoryFileSystem fs;
		private readonly InMemoryDirectoryEntry directory;
		private readonly string name;

		/// <inheritdoc />
		public string Name => name;

		/// <inheritdoc />
		public IDirectoryEntry Parent => directory;

		/// <inheritdoc />
		public bool CanExecute => false;

		public InMemoryFileEntry(InMemoryFileSystem fs, InMemoryDirectoryEntry directory, string name)
		{
			this.fs = fs;
			this.directory = directory;
			this.name = name;
		}
		
		
		/// <inheritdoc />
		public bool TryDelete(IUser user)
		{
			return true;
		}

		/// <inheritdoc />
		public bool TryOpenRead(IUser user, out Stream? stream)
		{
			// TODO
			stream = Stream.Null;
			return false;
		}

		/// <inheritdoc />
		public bool TryOpenWrite(IUser user, out Stream? stream)
		{
			// TODO
			stream = Stream.Null;
			return false;
		}

		/// <inheritdoc />
		public bool TryOpenWriteAppend(IUser user, out Stream? stream)
		{
			// TODO
			stream = Stream.Null;
			return false;
		}

		/// <inheritdoc />
		public Task<bool> TryExecute(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			return Task.FromResult(false);;
		}
	}
}