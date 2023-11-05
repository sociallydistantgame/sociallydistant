#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;

namespace Core
{
	public static class SociallyDistantUtility
	{
		public static bool IsPosixName(string? text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return false;

			for (var i = 0; i < text.Length; i++)
			{
				char character = text[i];

				if (char.IsWhiteSpace(character))
					return false;

				if (char.IsDigit(character) && i == 0)
					return false;

				if (character == '-' && i == 0)
					return false;

				if (!char.IsLetterOrDigit(character) && character != '_' && character != '-')
					return false;
			}

			return true;
		}

		public static string GetFriendlyFileSize(ulong numberOfBytes)
		{
			var fractionalValue = (double) numberOfBytes;

			var units = new string[]
			{
				"bytes",
				"KB",
				"MB",
				"GB",
				"TB"
			};

			var sb = new StringBuilder();

			for (var i = 0; i < units.Length; i++)
			{
				sb.Length = 0;
				sb.Append(fractionalValue.ToString("0.0"));
				sb.Append(" ");
				sb.Append(units[i]);

				if (fractionalValue < 1024)
					break;

				fractionalValue /= 1024;
			}

			return sb.ToString();
		}
		
		public static string GetGenderDisplayString(Gender gender)
		{
			// TODO: i18n
			return gender switch
			{
				Gender.Male => "He / Him / His",
				Gender.Female => "She / Her / Her's",
				Gender.Unknown => "They / Them / Their",
				_ => "<unknown>"
			};
		}

		public static IEnumerable<SystemVolume> GetSystemDiskDrives()
		{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
			return GetSystemDiskDrives_Win32();
#else
			return GetSystemDiskDrives_Posix();
#endif
		}
		
		#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

		private static IEnumerable<SystemVolume> GetSystemDiskDrives_Win32()
		{
			DriveInfo[] drives = DriveInfo.GetDrives();

			var driveName = new StringBuilder(261);
			var fsName = new StringBuilder(261);
			
			foreach (DriveInfo drive in drives)
			{
				if (!drive.IsReady)
					continue;

				string root = drive.RootDirectory.FullName;

				if (!GetVolumeInformation(
					    root,
					    driveName,
					    driveName.Capacity,
					    out uint serialNumber,
					    out uint maxComponentLength,
					    out FileSystemFeature features,
					    fsName,
					    fsName.Capacity
				    ))
					continue;

				driveName.Append($" ({drive.Name})");
				
				yield return new SystemVolume(root, driveName.ToString(), fsName.ToString(), (ulong) drive.TotalFreeSpace, (ulong) drive.TotalSize, drive.DriveType);
			}
		}
		
		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetVolumeInformation(
			string rootPathName,
			StringBuilder volumeNameBuffer,
			int volumeNameSize,
			out uint volumeSerialNumber,
			out uint maximumComponentLength,
			out FileSystemFeature fileSystemFlags,
			StringBuilder fileSystemNameBuffer,
			int nFileSystemNameSize
		);

		[Flags]
		private enum FileSystemFeature : uint
		{
			/// <summary>
			/// The file system preserves the case of file names when it places a name on disk.
			/// </summary>
			CasePreservedNames = 2,

			/// <summary>
			/// The file system supports case-sensitive file names.
			/// </summary>
			CaseSensitiveSearch = 1,

			/// <summary>
			/// The specified volume is a direct access (DAX) volume. This flag was introduced in Windows 10, version 1607.
			/// </summary>
			DaxVolume = 0x20000000,

			/// <summary>
			/// The file system supports file-based compression.
			/// </summary>
			FileCompression = 0x10,

			/// <summary>
			/// The file system supports named streams.
			/// </summary>
			NamedStreams = 0x40000,

			/// <summary>
			/// The file system preserves and enforces access control lists (ACL).
			/// </summary>
			PersistentACLS = 8,

			/// <summary>
			/// The specified volume is read-only.
			/// </summary>
			ReadOnlyVolume = 0x80000,

			/// <summary>
			/// The volume supports a single sequential write.
			/// </summary>
			SequentialWriteOnce = 0x100000,

			/// <summary>
			/// The file system supports the Encrypted File System (EFS).
			/// </summary>
			SupportsEncryption = 0x20000,

			/// <summary>
			/// The specified volume supports extended attributes. An extended attribute is a piece of
			/// application-specific metadata that an application can associate with a file and is not part
			/// of the file's data.
			/// </summary>
			SupportsExtendedAttributes = 0x00800000,

			/// <summary>
			/// The specified volume supports hard links. For more information, see Hard Links and Junctions.
			/// </summary>
			SupportsHardLinks = 0x00400000,

			/// <summary>
			/// The file system supports object identifiers.
			/// </summary>
			SupportsObjectIDs = 0x10000,

			/// <summary>
			/// The file system supports open by FileID. For more information, see FILE_ID_BOTH_DIR_INFO.
			/// </summary>
			SupportsOpenByFileId = 0x01000000,

			/// <summary>
			/// The file system supports re-parse points.
			/// </summary>
			SupportsReparsePoints = 0x80,

			/// <summary>
			/// The file system supports sparse files.
			/// </summary>
			SupportsSparseFiles = 0x40,

			/// <summary>
			/// The volume supports transactions.
			/// </summary>
			SupportsTransactions = 0x200000,

			/// <summary>
			/// The specified volume supports update sequence number (USN) journals. For more information,
			/// see Change Journal Records.
			/// </summary>
			SupportsUsnJournal = 0x02000000,

			/// <summary>
			/// The file system supports Unicode in file names as they appear on disk.
			/// </summary>
			UnicodeOnDisk = 4,

			/// <summary>
			/// The specified volume is a compressed volume, for example, a DoubleSpace volume.
			/// </summary>
			VolumeIsCompressed = 0x8000,

			/// <summary>
			/// The file system supports disk quotas.
			/// </summary>
			VolumeQuotas = 0x20
		}
	}
	
	#else
	private static IEnumerable<SystemVolume> GetSystemDiskDrives_Posix()
	{
		return DriveInfo.GetDrives()
			.Where(x => x.IsReady)
			.Select(x => new SystemVolume(x))
			.ToArray();
	}
	#endif
	
}