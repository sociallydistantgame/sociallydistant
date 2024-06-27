#nullable enable

using System;
using Core.Config;
using GamePlatform;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Core;
using UnityEngine;
using UnityExtensions;

namespace Audio
{
	public class AudioSettingsHandler : MonoBehaviour
	{
		private GameManager gameManager = null!;
		private float sfxVolume;
		private float bgmVolume;
		private float dialogueVolume;
		private IDisposable? settingsObserver;
		private AudioSettings? audioSettings;
		
		
		private void Awake()
		{
			gameManager = GameManager.Instance;
			
			this.AssertAllFieldsAreSerialized(typeof(AudioSettingsHandler));
		}

		private void Start()
		{
			audioSettings = gameManager.SettingsManager.RegisterSettingsCategory<AudioSettings>();
			settingsObserver = gameManager.SettingsManager.ObserveChanges(OnSettingsChanged);
		}

		private void OnDestroy()
		{
			settingsObserver?.Dispose();
			settingsObserver = null;
			
			if (audioSettings == null)
				return;
			
			gameManager.SettingsManager.UnregisterSettingsCategory(audioSettings);
			audioSettings = null;
		}

		private void UpdateAudioSettings()
		{
			if (AudioManager.Current != null)
			{
				AudioManager.Current.MusicVolume = bgmVolume;
				AudioManager.Current.SoundsVolume = sfxVolume;
			}
		}

		private void OnSettingsChanged(ISettingsManager settings)
		{
			if (audioSettings == null)
				return;
            
			this.sfxVolume = audioSettings.SfxVolume;
			this.bgmVolume = audioSettings.MusicVolume;

			this.UpdateAudioSettings();
		}
	}
}