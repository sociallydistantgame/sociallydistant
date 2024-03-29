using System;
using Core.Config;
using GamePlatform;
using UnityEngine;
using UnityExtensions;

namespace UI.Widgets.Settings
{
	public abstract class SettingsListener : MonoBehaviour
	{
		[SerializeField]
		private GameManagerHolder gameManagerHolder = null!;
		
		private IDisposable? settingsObserver;
		
		protected virtual void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SettingsListener));
		}

		protected virtual async void OnEnable()
		{
			BindToSettingsUpdates();
		}

		protected virtual void Update()
		{
			// In case we couldn't bind to settings updates during OnEnable, keep trying
			// aggressively until we either can or we get disabled.
			if (settingsObserver != null)
				return;

			BindToSettingsUpdates();
		}
		
		protected virtual void OnDisable()
		{
			settingsObserver?.Dispose();
		}

		protected abstract void OnSettingsChanged(ISettingsManager settingsManager);

		private void BindToSettingsUpdates()
		{
			if (gameManagerHolder.Value == null)
				return;
			
			settingsObserver = gameManagerHolder.Value.SettingsManager.ObserveChanges(OnSettingsChanged);
		}
	}
}