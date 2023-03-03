using UnityEngine;
using Utility;

namespace GamePlatform
{
	public class GamePlatformBootstrap : MonoBehaviour
	{
		[Header("Holder")]
		[SerializeField]
		private GamePlatformHolder platformHolder = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(GamePlatformBootstrap));
			platformHolder.Value = new OfflineGamePlatform();
		}
	}
}