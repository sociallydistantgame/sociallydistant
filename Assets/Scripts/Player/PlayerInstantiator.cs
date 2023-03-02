#nullable enable

using System;
using System.Collections.Generic;
using Architecture;
using GameplaySystems.GameManagement;
using OS.Devices;
using OS.FileSystems;
using UI.Backdrop;
using UnityEngine;
using Utility;

namespace Player
{
	public class PlayerInstantiator : MonoBehaviour
	{
		[Header("Player Instance")]
		[SerializeField]
		private PlayerInstanceHolder playerInstanceHolder = null!;

		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
		[SerializeField]
		private DeviceCoordinator deviceCoordinator = null!;

		[Header("File System Table")]
		[SerializeField]
		private FileSystemTableAsset fstab = null!;
		
		[Header("Prefabs")]
		[SerializeField]
		private GameObject uiRootPrefab = null!;

		[SerializeField]
		private GameObject backdropPrefab = null!;

		[SerializeField]
		private GameObject desktopPrefab = null!;
		
		[SerializeField]
		private GameObject windowManagerPrefab = null!;

		[Header("Environment")]
		[SerializeField]
		private EnvironmentVariablesAsset environmentVariables = null!;

		[Header("Backdrop")]
		[SerializeField]
		private Texture2D defaultBackdrop = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(PlayerInstantiator));
		}

		private void Start()
		{
			var playerComputer = new PlayerComputer(gameManager.Value);
			var player = new PlayerInstance();

			FileSystemTable.MountFileSystemsToComputer(playerComputer, fstab);
			
			player.Computer = playerComputer;
			player.OsInitProcess = deviceCoordinator.SetUpComputer(playerComputer);

			// Copy environment vars to the init process
			foreach (KeyValuePair<string, string> pair in this.environmentVariables)
			{
				player.OsInitProcess.Environment[pair.Key] = pair.Value;
			}
			
			desktopPrefab.SetActive(false);

			GameObject uiRootGameObject = Instantiate(uiRootPrefab);
			GameObject backdropGameObject = Instantiate(backdropPrefab, uiRootGameObject.transform);
			GameObject desktopGameObject = Instantiate(desktopPrefab, uiRootGameObject.transform);
			GameObject windowManagerGameObject = Instantiate(windowManagerPrefab, uiRootGameObject.transform);

			desktopPrefab.SetActive(true);
			
			player.UiRoot = uiRootGameObject;
			
			backdropGameObject.MustGetComponent(out player.BackdropController);
			desktopGameObject.MustGetComponent(out player.Desktop);
			windowManagerGameObject.MustGetComponent(out player.WindowManager);

			// set the default backdrop
			player.BackdropController.SetBackdrop(new BackdropSettings(Color.white, defaultBackdrop));
			
			this.playerInstanceHolder.Value = player;
		}

		private void OnDestroy()
		{
			PlayerInstance player = playerInstanceHolder.Value;

			deviceCoordinator.ForgetComputer(player.Computer);
			
			Destroy(player.WindowManager.gameObject);
			Destroy(player.BackdropController.gameObject);
			Destroy(player.UiRoot);

			playerInstanceHolder.Value = default;
		}
	}
}