#nullable enable

using System;
using Core;
using GamePlatform;
using Modding;
using UnityEngine;
using UnityExtensions;
using Utility;
using UniRx;

namespace GameplaySystems.Networld
{
	public class NetworldBootstrap : MonoBehaviour
	{
		private CoreRouter? coreRouter;
		private NetworkUpdateHook networkUpdateHook;

		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
		[SerializeField]
		private WorldManagerHolder worldHolder = null!;
		
		[SerializeField]
		private NetworkSimulationHolder holder = null!;

		private IDisposable? gameModeObserver;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(NetworldBootstrap));

			var worldHostResolver = new WorldHostResolver(worldHolder);
			
			this.coreRouter = new CoreRouter(worldHostResolver);
			this.holder.Value = new NetworkSimulationController(coreRouter);
			
			networkUpdateHook = new NetworkUpdateHook(worldHolder);
		}

		private void Start()
		{
			gameManager.Value!.ScriptSystem.RegisterHookListener("AfterWorldStateUpdate", networkUpdateHook);
			gameModeObserver = gameManager.Value!.GameModeObservable.Subscribe(OnGameModeChanged);
		}

		private void OnDestroy()
		{
			this.holder.Value?.DisableSimulation();
			
			gameModeObserver?.Dispose();
			gameManager.Value!.ScriptSystem.UnregisterHookListener("AfterWorldStateUpdate", networkUpdateHook);
		}
		
		private void OnGameModeChanged(GameMode newGameMode)
		{
			switch (newGameMode)
			{
				case GameMode.OnDesktop:
				case GameMode.InMission:
				case GameMode.LockScreen:
					this.holder.Value?.EnableSimulation();
					break;
				default:
					this.holder.Value?.DisableSimulation();
					break;
			}
		}
	}
}