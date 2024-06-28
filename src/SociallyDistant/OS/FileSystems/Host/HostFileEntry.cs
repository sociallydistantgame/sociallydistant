#nullable enable
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.OS.FileSystems.Host
{
	public class HostFileEntry : IFileEntry
	{
		private readonly string hostFile;

		/// <inheritdoc />
		public string Name => Path.GetFileName(hostFile);

		/// <inheritdoc />
		public bool CanExecute => false;
		
		/// <inheritdoc />
		public IDirectoryEntry Parent { get; }

		public HostFileEntry(IDirectoryEntry parent, string fullPath)
		{
			this.Parent = parent;
			this.hostFile = fullPath;
		}
		
		/// <inheritdoc />
		public bool TryDelete(IUser user)
		{
			File.Delete(hostFile);
			return true;
		}

		/// <inheritdoc />
		public bool TryOpenRead(IUser user, out Stream? stream)
		{
			stream = File.OpenRead(hostFile);
			return true;

		}

		/// <inheritdoc />
		public bool TryOpenWrite(IUser user, out Stream? stream)
		{
			stream = File.OpenWrite(hostFile);
			return true;
		}

		/// <inheritdoc />
		public bool TryOpenWriteAppend(IUser user, out Stream? stream)
		{
			stream = File.Open(hostFile, FileMode.Append);
			return true;
		}

		/// <inheritdoc />
		public Task<bool> TryExecute(ISystemProcess process, ITextConsole console, string[] arguments)
		{
			return Task.FromResult(false);
		}
	}
}