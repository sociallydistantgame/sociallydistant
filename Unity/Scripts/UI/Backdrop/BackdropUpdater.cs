#nullable enable
using System;
using Core;
using GamePlatform;
using UnityEngine;
using UnityExtensions;
using UniRx;

namespace UI.Backdrop
{
	public class BackdropUpdater : MonoBehaviour
	{
		[Header("Backdrops")]
		
		private Texture2D dayTime = null!;
		
		
		private Texture2D nightTime = null!;
		
		
		private Texture2D dayTimePanic = null!;
		
		
		private Texture2D nightTimePanic = null!;
		
		private IWorldManager worldManager = null!;
		private GameManager gameManager = null!;
		private GameMode gameMode;
		private bool isPanicking = false;
		private bool isNightTime = false;
		private BackdropController backdrop = null!;
		private IDisposable? gameModeObserver;
		
		private void Awake()
		{
			gameManager = GameManager.Instance;
			worldManager = gameManager.WorldManager;
			
			this.MustGetComponent(out backdrop);
		}
		
		private void Start()
		{
			gameModeObserver = gameManager.GameModeObservable.Subscribe(OnGameModeChanged);
		}
		
		private void OnDestroy()
		{
			gameModeObserver?.Dispose();
		}

		private void Update()
		{
			if (gameMode != GameMode.OnDesktop && gameMode != GameMode.InMission)
			{
				if (isNightTime && !isPanicking)
					return;

				isNightTime = true;
				isPanicking = false;
				UpdateBackdrop();
				return;
			}

			DateTime timeOfDay = worldManager.World.GlobalWorldState.Value.Now;

			bool day = timeOfDay.Hour >= 7 && timeOfDay.Hour < 19;
			bool night = !day;

			if (isNightTime == night)
				return;

			isNightTime = night;
			UpdateBackdrop();
		}
		
		private void UpdateBackdrop()
		{
			bool night = isNightTime;
			bool panic = isPanicking;

			Texture2D? texture = null;

			if (panic)
			{
				texture = night ? nightTimePanic : dayTimePanic;
			}
			else
			{
				texture = night ? nightTime : dayTime;
			}

			this.backdrop.SetBackdrop(new BackdropSettings(Color.white, texture));
		}

		private void OnGameModeChanged(GameMode newGameMode)
		{
			gameMode = newGameMode;
		}
	}
}