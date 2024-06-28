#nullable enable
namespace SociallyDistant.Core.Core;

public class SystemVolume
{
	private string label;
	private string path;
	private string filesystemName;
	private ulong totalSpace;
	private ulong freeSpace;
	private DriveType driveType;

	public string Path => path;
	public string VolumeLabel => label;
	public string FileSystemType => filesystemName;
	public ulong FreeSpace => freeSpace;
	public ulong TotalSpace => totalSpace;
	public DriveType DriveType => driveType;
	
	internal SystemVolume(string path, string label, string fs, ulong freeSpace, ulong totalSpace, DriveType driveType)
	{
		this.path = path;
		this.label = label;
		this.filesystemName = fs;
		this.totalSpace = totalSpace;
		this.freeSpace = freeSpace;
		this.driveType = driveType;
	}

	public SystemVolume(DriveInfo driveInfo) :
		this(
			driveInfo.VolumeLabel,
			driveInfo.RootDirectory.Name,
			driveInfo.DriveFormat,
			(ulong) driveInfo.TotalFreeSpace,
			(ulong) driveInfo.TotalSize,
			driveInfo.DriveType
		)
	{
		
	}
}