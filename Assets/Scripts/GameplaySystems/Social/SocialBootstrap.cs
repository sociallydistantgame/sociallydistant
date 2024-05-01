#nullable enable
using UnityEngine;
using UnityExtensions;

namespace GameplaySystems.Social
{
	public class SocialBootstrap : MonoBehaviour
	{
		[SerializeField]
		private SocialServiceHolder socialHolder = null!;

		[SerializeField]
		private SocialService prefab = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SocialBootstrap));
			socialHolder.Value = Instantiate(prefab);
		}
	}
}