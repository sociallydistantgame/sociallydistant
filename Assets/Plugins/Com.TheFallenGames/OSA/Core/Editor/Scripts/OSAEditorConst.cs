using Com.TheFallenGames.OSA.Editor.Util;
using System.IO;

namespace Com.TheFallenGames.OSA.Editor
{
	public class OSAEditorConst
	{
		/// <summary>The path of the OSA folder relative to Unity's project folder</summary>
		public static string OSA_PATH_IN_PROJECT 
		{ 
			get
			{
				// Expecting the tracker asset to be under '[OSA dir]/Core/Editor/', so we get the parent of the parent folder of that to return the OSA's folder.
				var dir = PluginPathTracker.DirectoryPathInAssets;
				dir = Path.GetDirectoryName(dir);
				dir = Path.GetDirectoryName(dir);
				return dir.Replace("\\", "/");
			} 
		}
	}
}
