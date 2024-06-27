#nullable enable
using UnityEngine;
using UnityExtensions;

namespace GameplaySystems.Missions
{
	public sealed class MissionManagerBootstrap : MonoBehaviour
	{
		[SerializeField]
		private MissionManager prefab = null!;

		private MissionManager missionManager;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(MissionManagerBootstrap));
			missionManager = Instantiate(prefab);
		}
	}
}