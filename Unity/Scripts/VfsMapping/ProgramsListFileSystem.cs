#nullable enable
using Architecture;
using UnityEngine;

namespace VfsMapping
{
	[CreateAssetMenu(menuName = "ScriptableObject/FS/Asset Mappings/UGUI Programs Directory")]
	public class ProgramsListFileSystem : AssetListVfsMap<UguiProgram, UguiProgramBinaryFile>
	{
		
	}
}