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

		[SerializeField]
		private GameObject windowManagerPrefab = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(PlayerInstantiator));
		}

		private void Start()
		{
			var player = new PlayerInstance();

			GameObject uiRootGameObject = Instantiate(uiRootPrefab);
			GameObject backdropGameObject = Instantiate(backdropPrefab, uiRootGameObject.transform);
			GameObject windowManagerGameObject = Instantiate(windowManagerPrefab, uiRootGameObject.transform);

			player.UiRoot = uiRootGameObject;
			
			backdropGameObject.MustGetComponent(out player.BackdropController);
			windowManagerGameObject.MustGetComponent(out player.WindowManager);

			this.playerInstanceHolder.Value = player;

			this.playerInstanceHolder.Value.WindowManager.FallbackWorkspace.CreateWindow("Ritchie");
		}

		private void OnDestroy()
		{
			PlayerInstance player = playerInstanceHolder.Value;

			Destroy(player.WindowManager.gameObject);
			Destroy(player.BackdropController.gameObject);
			Destroy(player.UiRoot);

			playerInstanceHolder.Value = default;
		}
	}
}