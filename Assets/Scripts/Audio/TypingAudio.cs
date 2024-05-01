#nullable enable
using System;
using Core.Config;
using GamePlatform;
using TMPro;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Core;
using TrixelCreative.TrixelAudio.Data;
using UnityEngine;
using UnityExtensions;

namespace Audio
{
	[RequireComponent(typeof(TMP_InputField))]
	public sealed class TypingAudio : MonoBehaviour
	{
		[SerializeField]
		private SoundEffectAsset typingSound = null!;
		
		private TMP_InputField inputField;
		private IDisposable? settingsObserver;
		private bool soundEnabled = true;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TypingAudio));
			this.MustGetComponent(out inputField);
		}

		private void OnEnable()
		{
			inputField.onValueChanged.AddListener(OnValueChanged);
			settingsObserver = GameManager.Instance.SettingsManager.ObserveChanges(OnSystemSettingsChanged);
		}

		private void OnDisable()
		{
			settingsObserver?.Dispose();
			inputField.onValueChanged.RemoveListener(OnValueChanged);
		}

		private void OnValueChanged(string value)
		{
			PlayTypingSound();
		}
		
		private void OnSystemSettingsChanged(ISettingsManager settings)
		{
			var audioSettings = new AudioSettings(settings);
			soundEnabled = audioSettings.TypingSounds;
		}

		public void PlayTypingSound()
		{
			if (!soundEnabled)
				return;
			
			AudioManager.PlaySound(typingSound);
		}
	}
}