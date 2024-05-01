#nullable enable

using OS.FileSystems;
using UnityEngine;

namespace Architecture
{
	public abstract class FileSystemAsset :
		ScriptableObject,
		IFileSystemProvider
	{
		/// <inheritdoc />
		public abstract IFileSystem GetFileSystem();
	}
}