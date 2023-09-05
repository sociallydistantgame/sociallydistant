using UnityEngine;
using UnityExtensions;
using Utility;

namespace GameplaySystems.Hacking
{
	public class HackingSystemBootstrap : MonoBehaviour
	{
		[SerializeField]
		private HackingHolder holder = null!;

		[SerializeField]
		private HackingSystem prefab = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(HackingSystemBootstrap));
			holder.Value = Instantiate(prefab);
		}
	}
}