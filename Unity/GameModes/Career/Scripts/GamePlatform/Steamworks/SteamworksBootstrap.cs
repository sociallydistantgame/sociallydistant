#nullable enable
using System;
using GamePlatform;
using Steamworks;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace GameModes.Career.Scripts.GamePlatform.Steamworks
{
	public class SteamworksBootstrap : MonoBehaviour
	{
		private bool isSteamRunning;
		
		[Header("Holder")]
		
		private GamePlatformHolder platformHolder = null!;

		[Header("Configuration")]
		
		private uint steamAppId;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SteamworksBootstrap));

			try
			{
				SteamClient.Init(steamAppId);
				isSteamRunning = true;
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Steamworks initialization failed, see upcoming exception in the log. OfflineGamePlatform from the open-source version of the game will be used, online functionality will not work.");
				Debug.LogException(ex);
			}

			if (isSteamRunning)
			{
				Debug.Log("Overriding current GamePlatform implementation with Steamworks!");
				platformHolder.Value = new SteamworksGamePlatform();
			}
		}

		private void OnDestroy()
		{
			if (isSteamRunning)
			{
				SteamClient.Shutdown();
				platformHolder.Value = new OfflineGamePlatform();
			}
		}
	}
}