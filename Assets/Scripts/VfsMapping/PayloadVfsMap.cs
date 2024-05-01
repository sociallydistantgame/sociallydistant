#nullable enable
using GameplaySystems.Hacking;
using UnityEngine;

namespace VfsMapping
{
	[CreateAssetMenu(menuName = "ScriptableObject/FS/Asset Mappings/Payloads Directory")]
	public class PayloadVfsMap : UnlockableAssetListVfsMap<PayloadAsset, PayloadFile>
	{
		
	}
}