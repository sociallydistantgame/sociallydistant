#nullable enable
using System;
using System.IO;
using Architecture;
using Core;
using MimeTypes;
using OS.Devices;
using OS.FileSystems;
using Player;
using UnityEngine;
using System.Threading.Tasks;

namespace UI.Applications.FileManager
{
	[CreateAssetMenu(menuName = "ScriptableObject/File Manager/File Association Map")]
	public class FileAssociationMap : ScriptableObject
	{
		
		private PlayerInstanceHolder player = null!;

		
		private MimeTypeProgramMap mimeTypeAssociations = new MimeTypeProgramMap();
		
		public async Task<ISystemProcess?> OpenFile(ISystemProcess parentProcess, string filePath)
		{
			IVirtualFileSystem vfs = parentProcess.User.Computer.GetFileSystem(parentProcess.User);

			if (!vfs.FileExists(filePath))
				return null;
			
            IUser user = parentProcess.User;

            string fileName = PathUtility.GetFileName(filePath);

			string withoutExtension = Path.GetFileNameWithoutExtension(fileName);
			string extension = fileName.Substring(withoutExtension.Length);

			string mimeType = MimeTypeMap.GetMimeType(extension);

			if (!this.mimeTypeAssociations.TryGetValue(mimeType, out UguiProgram? program))
				return null;

			ISystemProcess? p = parentProcess;
			while (p is not LoginProcess)
				p = p.Parent;

			if (p == null)
				return null;

			if (player.Value.UiManager == null)
				return null;

			if (player.Value.UiManager.Desktop == null)
				return null;
			
			return await player.Value.UiManager.Desktop.OpenProgram(program, new[] { filePath }, null, null);
		}
	}
}