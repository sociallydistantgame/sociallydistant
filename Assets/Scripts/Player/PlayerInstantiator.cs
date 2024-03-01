#nullable enable

using System.Collections.Generic;
using Architecture;
using Core.Scripting;
using GamePlatform;
using GameplaySystems.Networld;
using OS.Devices;
using OS.FileSystems;
using UI.PlayerUI;
using UnityEngine;
using UnityExtensions;

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
		private GameObject uiRootPrefab = null!;

		[Header("Environment")]
		[SerializeField]
		private OperatingSystemScript loginScript = null!;
		
		[SerializeField]
		private EnvironmentVariablesAsset environmentVariables = null!;

		[Header("Backdrop")]
		[SerializeField]
		private Texture2D defaultBackdrop = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(PlayerInstantiator));
		}

		private async void Start()
		{
			if (gameManager.Value != null)
				await gameManager.Value.WaitForModulesToLoad();
			
			// Create a ghost LAN for the player
			LocalAreaNetwork playerLan = networkSimulation.Value.CreateLocalAreaNetwork();


			var fileOverrider = new PlayerFileOverrider();
			var playerComputer = new PlayerComputer(gameManager.Value, playerLan, fileOverrider, this.loginScript, fstab);
			var player = new PlayerInstance();

			player.FileOverrider = fileOverrider;
			player.PlayerLan = playerLan;
			
			player.Computer = playerComputer;
			player.OsInitProcess = deviceCoordinator.SetUpComputer(playerComputer, loginScript);

			playerComputer.SetInitProcess(player.OsInitProcess);
			
			// Copy environment vars to the init process
			foreach (KeyValuePair<string, string> pair in this.environmentVariables)
			{
				player.OsInitProcess.Environment[pair.Key] = pair.Value;
			}
			
			GameObject uiRootGameObject = Instantiate(uiRootPrefab);
            
			player.UiRoot = uiRootGameObject;
			player.UiManager = uiRootGameObject.MustGetComponent<UiManager>();
			
			this.playerInstanceHolder.Value = player;
		}
		
		private void OnDestroy()
		{
			PlayerInstance player = playerInstanceHolder.Value;

			deviceCoordinator.ForgetComputer(player.Computer);
			
			playerInstanceHolder.Value = default;
		}
	}
}