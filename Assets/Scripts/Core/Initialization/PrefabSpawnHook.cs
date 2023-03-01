#nullable enable
using UnityEngine;

namespace Core.Initialization
{
	[CreateAssetMenu(menuName = "ScriptableObject/Initialization Hooks/Prefab Spawn Initialization Hook")]
	public class PrefabSpawnHook : InitializationHook
	{
		[SerializeField]
		private GameObject prefab = null!;

		/// <inheritdoc />
		public override void RunHook()
		{
			Instantiate(prefab);
		}
	}
}