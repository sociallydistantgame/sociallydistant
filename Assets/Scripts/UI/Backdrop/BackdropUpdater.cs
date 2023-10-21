#nullable enable
using System;
using Core;
using GamePlatform;
using UI.Themes.ThemeData;
using UI.Themes.ThemedElements;
using UI.Theming;
using UnityEngine;
using UnityExtensions;
using UniRx;

namespace UI.Backdrop
{
	public class BackdropUpdater : ShellElement
	{
		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
		[SerializeField]
		private WorldManagerHolder worldManager = null!;

		private GameMode gameMode;
		private bool isNightTime = false;
		private BackdropController backdrop = null!;
		private IDisposable? gameModeObserver;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.MustGetComponent(out backdrop);
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();

			if (gameManager.Value == null)
				return;

			gameModeObserver = gameManager.Value.GameModeObservable.Subscribe(OnGameModeChanged);
		}

		/// <inheritdoc />
		protected override void OnDestroy()
		{
			base.OnDestroy();
			gameModeObserver?.Dispose();
		}

		private void Update()
		{
			if (worldManager.Value == null ||
			    (gameMode != GameMode.OnDesktop && gameMode != GameMode.InMission))
			{
				if (!isNightTime)
					return;

				isNightTime = false;
				NotifyThemePropertyChanged();
				return;
			}

			DateTime timeOfDay = worldManager.Value.World.GlobalWorldState.Value.Now;

			bool day = timeOfDay.Hour >= 7 && timeOfDay.Hour < 19;
			bool night = !day;

			if (isNightTime == night)
				return;

			isNightTime = night;
			NotifyThemePropertyChanged();
		}

		/// <inheritdoc />
		protected override void OnUpdateTheme(OperatingSystemTheme theme)
		{
			BackdropStyle backdropStyle = theme.BackdropStyle;

			bool dark = Provider.UseDarkMode;
			bool night = isNightTime;

			ThemeGraphic graphic = night
				? backdropStyle.NightTime
				: backdropStyle.DayTime;

			Color tint = theme.TranslateColor(graphic.Color, dark);
			
			this.backdrop.SetBackdrop(new BackdropSettings(tint, graphic.Texture));
		}

		private void OnGameModeChanged(GameMode newGameMode)
		{
			gameMode = newGameMode;
		}
	}
}