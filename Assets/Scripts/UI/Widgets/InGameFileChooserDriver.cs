#nullable enable
using System.Collections.Generic;
using Core;
using OS.Devices;
using OS.FileSystems;

namespace UI.Widgets
{
	public class InGameFileChooserDriver : IFileChooserDriver
	{
		private readonly ISystemProcess process;
		private readonly IUser user;
		private readonly IVirtualFileSystem fs;

		public InGameFileChooserDriver(ISystemProcess process)
		{
			this.process = process;
			this.user = process.User;
			this.fs = user.Computer.GetFileSystem(user);
		}
		
		/// <inheritdoc />
		public bool DirectoryExists(string path)
		{
			return fs.DirectoryExists(path);
		}

		/// <inheritdoc />
		public bool FileExists(string path)
		{
			return fs.FileExists(path);
		}

		/// <inheritdoc />
		public IEnumerable<string> GetSubDirectories(string path)
		{
			return fs.GetDirectories(path);
		}

		/// <inheritdoc />
		public IEnumerable<string> GetFiles(string path)
		{
			return fs.GetFiles(path);
		}

		/// <inheritdoc />
		public IEnumerable<string> GetCommonPlacePaths()
		{
			yield return user.Home;

			yield return PathUtility.Combine(user.Home, "Desktop");
			yield return PathUtility.Combine(user.Home, "Documents");
			yield return PathUtility.Combine(user.Home, "Downloads");
			yield return PathUtility.Combine(user.Home, "Music");
			yield return PathUtility.Combine(user.Home, "Pictures");
			yield return PathUtility.Combine(user.Home, "Videos");
		}

		/// <inheritdoc />
		public IEnumerable<SystemVolume> GetSystemVolumes()
		{
			yield break;
		}
	}
}