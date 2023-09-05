#nullable enable

using System;
using Core.Config;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace Audio
{
	public class TrixelAudioSettingsHandler : MonoBehaviour
	{
		private float sfxVolume;
		private float bgmVolume;
		private float dialogueVolume;
		
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TrixelAudioSettingsHandler));
			Registry.Updated += UpdateAudioSettings;
		}

		private void UpdateAudioSettings()
		{
			Registry.GetValueOrSetDefault("trixel.audio.sfxVolume", 1, out sfxVolume);
			Registry.GetValueOrSetDefault("trixel.audio.bgmVolume", 1, out bgmVolume);
			Registry.GetValueOrSetDefault("trixel.audio.dialogueVolume", 1, out dialogueVolume);
		}

		private void Start()
		{
			UpdateAudioSettings();
		}
	}
}