#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using Core;
using UnityEngine;

namespace UI.Widgets
{
	public class HostFileChooserDriver : IFileChooserDriver
	{
		/// <inheritdoc />
		public string PathCombine(params string[] paths)
		{
			// just let .NET do it
			return Path.Combine(paths);
		}

		/// <inheritdoc />
		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		/// <inheritdoc />
		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		/// <inheritdoc />
		public IEnumerable<string> GetSubDirectories(string path)
		{
			return Directory.GetDirectories(path);
		}

		/// <inheritdoc />
		public IEnumerable<string> GetFiles(string path)
		{
			return Directory.GetFiles(path);
		}

		/// <inheritdoc />
		public IEnumerable<string> GetCommonPlacePaths()
		{
			if (Application.isEditor)
				yield return Application.dataPath.Replace('/', Path.DirectorySeparatorChar);;
			
			yield return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			yield return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			yield return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			yield return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
			yield return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			yield return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
			yield return Application.persistentDataPath.Replace('/', Path.DirectorySeparatorChar);
		}

		/// <inheritdoc />
		public IEnumerable<SystemVolume> GetSystemVolumes()
		{
			return SociallyDistantUtility.GetSystemDiskDrives();
		}
	}
}