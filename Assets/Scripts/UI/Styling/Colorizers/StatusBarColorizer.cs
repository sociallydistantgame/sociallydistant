#nullable enable

using System;
using Core;
using GamePlatform;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;
using UniRx;

namespace UI.Styling.Colorizers
{
	[ExecuteInEditMode]
	public class StatusBarColorizer : UIBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManagerHolder = null!;

		[Header("Colors")]
		[SerializeField]
		private Color normalBright;
		
		[SerializeField]
		private Color panicBright;
		
		[SerializeField]
		private Color normalDark;
		
		[SerializeField]
		private Color panicDark;

		[Header("Visualizer settings")]
		[SerializeField]
		private float panicPulsesPerSecond = 4;

		[SerializeField]
		private float panicPulseLength = 0.1f;

		[SerializeField]
		private float safetyPulseHold = 1;

		[SerializeField]
		private float safetyPulseDecay = 0.5f;

		private IDisposable? gameModeObserver;
		private IDisposable? panicObserver;
		private GameMode gameMode;
		private bool willPanic;
		private bool isPanicking;
		private Graphic graphic = null!;
		private float timeUntilNextPulse = 0;
		private float hold = 0;
		private float decay = 0;

		/// <inheritdoc />
		protected override void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(StatusBarColorizer));
			
			this.MustGetComponent(out graphic);
			
			base.Awake();
		}

		/// <inheritdoc />
		protected override void Start()
		{
			base.Start();

			if (gameManagerHolder.Value == null)
				return;

			panicObserver = gameManagerHolder.Value.PanicObservable.Subscribe(OnPanicChanged);
			gameModeObserver = gameManagerHolder.Value.GameModeObservable.Subscribe(OnGameModeChanged);
		}
		
		private void Update()
		{
			bool wasPanicking = isPanicking;

			isPanicking = willPanic;

			if (wasPanicking == isPanicking)
			{
				if (isPanicking)
				{
					if (decay > 0)
					{
						decay = Mathf.Clamp(decay - Time.deltaTime * panicPulseLength, 0, 1);

						if (decay <= 0)
						{
							float waitTime = 1f / panicPulsesPerSecond;
							timeUntilNextPulse = Mathf.Clamp(waitTime - panicPulseLength, 0, waitTime);
						}
					}

					if (timeUntilNextPulse > 0)
						timeUntilNextPulse -= Time.deltaTime;
					else
					{
						hold = 0;
						decay = 1;
					}
				}
				else
				{
					if (hold > 0)
						hold -= Time.deltaTime;
					else if (decay > 0)
						decay = Mathf.Clamp(decay - Time.deltaTime * safetyPulseDecay, 0, 1);
					
					
				}
			}
			else
			{
				if (isPanicking)
				{
					decay = 1;
					hold = 0;
					timeUntilNextPulse = 0;
				}
				else
				{
					hold = safetyPulseDecay;
					decay = 1;
				}
			}

			graphic.color = isPanicking
				? Color.Lerp(panicDark, panicBright, decay)
				: Color.Lerp(normalDark, normalBright, decay);
		}

		private void OnGameModeChanged(GameMode newGameMode)
		{
			this.gameMode = newGameMode;

			if (gameMode != GameMode.OnDesktop && gameMode != GameMode.InMission)
				willPanic = false;
		}
		
		private void OnPanicChanged(bool panic)
		{
			if (gameMode == GameMode.OnDesktop || gameMode == GameMode.InMission)
				willPanic = panic;
			else
				willPanic = false;
		}
	}
}