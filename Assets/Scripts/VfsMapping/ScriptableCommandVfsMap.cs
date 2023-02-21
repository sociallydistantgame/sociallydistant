#nullable enable
using Architecture;
using UnityEngine;

namespace VfsMapping
{
	[CreateAssetMenu(menuName = "ScriptableObject/FS/Asset Mappings/Scriptable Commands Directory")]
	public class ScriptableCommandVfsMap : AssetListVfsMap<ScriptableCommandBase, ScriptableCommandFileEntry>
	{
		
	}
}