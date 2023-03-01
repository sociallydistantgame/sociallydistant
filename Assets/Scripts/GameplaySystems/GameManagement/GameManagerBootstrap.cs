using Core;
using UnityEngine;
using Utility;

namespace GameplaySystems.GameManagement
{
	public class GameManagerBootstrap : MonoBehaviour
	{
		[Header("Holder")]
		[SerializeField]
		private GameManagerHolder holder = null!;

		[SerializeField]
		private WorldManagerHolder world = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(GameManagerBootstrap));
			holder.Value = new GameManager(world);
		}
	}
}