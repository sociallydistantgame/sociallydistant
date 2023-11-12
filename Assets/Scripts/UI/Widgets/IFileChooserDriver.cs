#nullable enable
using System.Collections.Generic;
using UI.Applications.FileManager;

namespace UI.Widgets
{
	public interface IFileChooserDriver
	{
		bool DirectoryExists(string path);
		bool FileExists(string path);
		
		IEnumerable<string> GetSubDirectories(string path);
		IEnumerable<string> GetFiles(string path);

		IEnumerable<string> GetCommonPlacePaths();
		IEnumerable<SystemVolume> GetSystemVolumes();
	}
}