#nullable enable
using System;
using OS.Devices;
using UnityEngine.EventSystems;

namespace OS.FileSystems
{
	public static class FileSystemTable
	{
		public static void MountFileSystemsToComputer(IComputer computer, IFileSystemTable fstab)
		{
			if (!computer.FindUserById(0, out IUser? rootUser) || rootUser==null || rootUser.PrivilegeLevel != PrivilegeLevel.Root)
				throw new InvalidOperationException("Computer does not have a root user, filesystems cannot be mounted.");

			IVirtualFileSystem vfs = computer.GetFileSystem(rootUser);
			foreach (IFileSystemTableEntry tableEntry in fstab.Entries)
			{
				IFileSystem fs = tableEntry.FileSystemProvider.GetFileSystem();
				vfs.Mount(tableEntry.Path, fs);
			}
		}
	}
}