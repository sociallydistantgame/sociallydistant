#nullable enable

using System;
using UnityEngine;
using Utility;

namespace Player
{
	public class PlayerInstantiator : MonoBehaviour
	{
		[Header("Player Instance")]
		[SerializeField]
		private PlayerInstanceHolder playerInstanceHolder = null!;

		[Header("Prefabs")]
		[SerializeField]
		private GameObject uiRootPrefab = null!;

		[SerializeField]
		private GameObject backdropPrefab = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(PlayerInstantiator));
		}

		private void Start()
		{
			var player = new PlayerInstance();

			GameObject uiRootGameObject = Instantiate(uiRootPrefab);
			GameObject backdropGameObject = Instantiate(backdropPrefab, uiRootGameObject.transform);

			player.UiRoot = uiRootGameObject;
			
			backdropGameObject.MustGetComponent(out player.BackdropController);

			this.playerInstanceHolder.Value = player;
		}

		private void OnDestroy()
		{
			PlayerInstance player = playerInstanceHolder.Value;

			Destroy(player.BackdropController.gameObject);
			Destroy(player.UiRoot);

			playerInstanceHolder.Value = default;
		}
	}
}