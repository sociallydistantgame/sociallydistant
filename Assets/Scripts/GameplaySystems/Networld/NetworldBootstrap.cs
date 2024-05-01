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
		[SerializeField]
		private NetworkSimulationHolder holder = null!;
		
		private IWorldManager worldHolder = null!;
		private CoreRouter? coreRouter;
		private NetworkUpdateHook networkUpdateHook;
		private GameManager gameManager = null!;
		private IDisposable? gameModeObserver;
		
		private void Awake()
		{
			gameManager = GameManager.Instance;
			worldHolder = gameManager.WorldManager;
			this.AssertAllFieldsAreSerialized(typeof(NetworldBootstrap));

			var worldHostResolver = new WorldHostResolver(worldHolder);
			
			this.coreRouter = new CoreRouter(worldHostResolver);
			this.holder.Value = new NetworkSimulationController(coreRouter);
			
			networkUpdateHook = new NetworkUpdateHook(worldHolder);
		}

		private void Start()
		{
			gameManager!.ScriptSystem.RegisterHookListener("AfterWorldStateUpdate", networkUpdateHook);
			gameModeObserver = gameManager.GameModeObservable.Subscribe(OnGameModeChanged);
		}

		private void OnDestroy()
		{
			this.holder.Value?.DisableSimulation();
			
			gameModeObserver?.Dispose();
			
			gameManager.ScriptSystem.UnregisterHookListener("AfterWorldStateUpdate", networkUpdateHook);
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