#nullable enable
using UnityEngine;
using UnityExtensions;

namespace GameplaySystems.Missions
{
	public sealed class MissionManagerBootstrap : MonoBehaviour
	{
		[SerializeField]
		private MissionManagerHolder holder = null!;

		[SerializeField]
		private MissionManager prefab = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(MissionManagerBootstrap));
			holder.Value = Instantiate(prefab);
		}

		private void OnDestroy()
		{
			if (holder.Value == null)
				return;
			
			Destroy(holder.Value.gameObject);
			holder.Value = null;
		}
	}
}