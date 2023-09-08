#nullable enable

using System;
using Core.Config;
using GamePlatform;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Core;
using UnityEngine;
using UnityExtensions;
using Utility;
using UniRx;
using UnityEngine.PlayerLoop;

namespace Audio
{
	public class TrixelAudioSettingsHandler : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField]
		private GameManagerHolder gameManager = null!;
		
		private float sfxVolume;
		private float bgmVolume;
		private float dialogueVolume;
		private IDisposable? settingsObserver;
		private TrixelAudioCore trixelAudio;
		private AudioSettings? audioSettings;
		
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TrixelAudioSettingsHandler));
			this.MustGetComponent(out trixelAudio);
		}

		private void OnEnable()
		{
			if (gameManager.Value == null)
				return;

			audioSettings = gameManager.Value.SettingsManager.RegisterSettingsCategory<AudioSettings>();
			settingsObserver = gameManager.Value.SettingsManager.ObserveChanges(OnSettingsChanged);
		}

		private void OnDisable()
		{
			settingsObserver?.Dispose();
			settingsObserver = null;

			if (gameManager.Value == null)
				return;

			if (audioSettings == null)
				return;
			
			gameManager.Value.SettingsManager.UnregisterSettingsCategory(audioSettings);
			audioSettings = null;
		}

		private void UpdateAudioSettings()
		{
			trixelAudio.Configuration.MusicMixer.audioMixer.SetFloat("MusicVolume", AudioUtility.PercentageToDb(bgmVolume));
			trixelAudio.Configuration.SoundEffectsMixer.audioMixer.SetFloat("GuiVolume", AudioUtility.PercentageToDb(sfxVolume));
			
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