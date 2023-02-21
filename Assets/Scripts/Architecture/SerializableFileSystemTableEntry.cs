#nullable enable
using System;
using OS.FileSystems;
using UnityEngine;

namespace Architecture
{
	[Serializable]
	public class SerializableFileSystemTableEntry : IFileSystemTableEntry
	{
		[SerializeField]
		private string path = "/";
		
		[SerializeField]
		private FileSystemAsset fileSystemAsset = null!;

		/// <inheritdoc />
		public string Path => path;

		/// <inheritdoc />
		public IFileSystemProvider FileSystemProvider => fileSystemAsset;
	}
}