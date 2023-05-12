#nullable enable

using System;
using System.Collections.Generic;
using Architecture;
using GameplaySystems.GameManagement;
using GameplaySystems.Networld;
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
		private NetworkSimulationHolder networkSimulation = null!;
		
		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
		[SerializeField]
		private DeviceCoordinator deviceCoordinator = null!;

		[Header("File System Table")]
		[SerializeField]
		private FileSystemTableAsset fstab = null!;

		[Header("Prefabs")]
		[SerializeField]
		private GameObject loginManagerPrefab = null!;
		
		[SerializeField]
		private GameObject uiRootPrefab = null!;

		[SerializeField]
		private GameObject backdropPrefab = null!;

		[SerializeField]
		private GameObject desktopPrefab = null!;
		
		[SerializeField]
		private GameObject windowManagerPrefab = null!;

		[SerializeField]
		private GameObject popoverLayerPrefab = null!;
		
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
			gameManager.Value.GameStarted += OnGameStart;
			gameManager.Value.GameEnded += OnGameEnded;
			
			// Create a ghost LAN for the player
			LocalAreaNetwork playerLan = networkSimulation.Value.CreateLocalAreaNetwork();


			var fileOverrider = new PlayerFileOverrider();
			var playerComputer = new PlayerComputer(gameManager.Value, playerLan, fileOverrider);
			var player = new PlayerInstance();

			player.FileOverrider = fileOverrider;
			player.PlayerLan = playerLan;
			
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
			GameObject loginManagerGameObject = Instantiate(loginManagerPrefab, uiRootGameObject.transform);
			GameObject windowManagerGameObject = Instantiate(windowManagerPrefab, uiRootGameObject.transform);
			GameObject popoverLayerGameObject = Instantiate(popoverLayerPrefab, uiRootGameObject.transform);

			desktopPrefab.SetActive(true);
			
			player.UiRoot = uiRootGameObject;
			
			backdropGameObject.MustGetComponent(out player.BackdropController);
			desktopGameObject.MustGetComponent(out player.Desktop);
			windowManagerGameObject.MustGetComponent(out player.WindowManager);
			loginManagerGameObject.MustGetComponent(out player.LoginManager);
			popoverLayerGameObject.MustGetComponent(out player.PopoverLayer);

			// set the default backdrop
			player.BackdropController.SetBackdrop(new BackdropSettings(Color.white, defaultBackdrop));
			
			this.playerInstanceHolder.Value = player;
		}

		private void OnGameEnded()
		{
			playerInstanceHolder.Value.Desktop.gameObject.SetActive(false);
			
			// We do this to force a rebuild of the player VFS such that we are no longer reading/writing in the save file.
			playerInstanceHolder.Value.Computer.SetPlayerUserName(gameManager.Value.CurrentPlayerName);
			
			FileSystemTable.MountFileSystemsToComputer(playerInstanceHolder.Value.Computer, fstab);
		}

		private void OnGameStart()
		{
			playerInstanceHolder.Value.Computer.SetPlayerUserName(gameManager.Value.CurrentPlayerName);
			FileSystemTable.MountFileSystemsToComputer(playerInstanceHolder.Value.Computer, fstab);
			playerInstanceHolder.Value.Desktop.gameObject.SetActive(true);
		}

		private void OnDestroy()
		{
			PlayerInstance player = playerInstanceHolder.Value;

			deviceCoordinator.ForgetComputer(player.Computer);
			
			playerInstanceHolder.Value = default;
		}
	}
}