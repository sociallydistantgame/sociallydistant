#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using OS.FileSystems;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu(menuName = "ScriptableObject/VFS/File System Table")]
	public class FileSystemTableAsset :
		ScriptableObject,
		IFileSystemTable
	{
		[SerializeField]
		private SerializableFileSystemTableEntry[] entries = Array.Empty<SerializableFileSystemTableEntry>();

		/// <inheritdoc />
		public IEnumerable<IFileSystemTableEntry> Entries
			=> entries.Where(x => !string.IsNullOrWhiteSpace(x.Path) && x.FileSystemProvider != null!);
	}
}