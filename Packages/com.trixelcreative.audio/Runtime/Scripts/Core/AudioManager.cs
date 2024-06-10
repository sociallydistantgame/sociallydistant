#nullable enable

using System;
using TrixelCreative.TrixelAudio.Data;
using UnityEngine;
using UnityEngine.Audio;

namespace TrixelCreative.TrixelAudio.Core
{
	public sealed class AudioManager : MonoBehaviour
	{
		[Header("Settings")]
		[SerializeField]
		[Tooltip("The maximum number of sounds that can be played at once. Increase this value if sounds sometimes refuse to play, decrease this value to save on memory.")]
		private int maximumSoundsAtOnce = 20;

		[SerializeField]
		[Tooltip("The primary AudioMixer asset used by your game. Volume settings on the AudioManager apply to this mixer's groups.")]
		private AudioMixer primaryAudioMixer = null!;
		
		[SerializeField]
		[Tooltip("The mixer group on which to play normal game sounds")]
		private AudioMixerGroup soundsGroup = null;

		[SerializeField]
		[Tooltip("The mixer group on which to play GUI navigation sounds")]
		private AudioMixerGroup guiSoundsGroup = null!;

		[SerializeField]
		[Tooltip("The mixer group on which to play speech/dialogue sounds.")]
		private AudioMixerGroup dialogueGrroup = null!;

		[SerializeField]
		[Tooltip("The mixer group on which to play background music.")]
		private AudioMixerGroup musicGroup = null!;

		[SerializeField]
		private string soundsVolumeParameter = string.Empty;
		
		[SerializeField]
		private string guiSoundsVolumeParameter = string.Empty;
		
		[SerializeField]
		private string dialogueVolumeParameter = string.Empty;
		
		[SerializeField]
		private string musicVolumeParameter = string.Empty;

		[Header("GUI")]
		[SerializeField]
		private UserInterfaceSoundSchemeAsset? guiSounds = null;
		
		private static AudioManager? current;
		private AudioSourcePool soundPool = null!;
		private SongPlayerCore songPlayer = null!;

		public static AudioManager? Current => current;
		
		public float SoundsVolume
		{
			get
			{
				if (primaryAudioMixer.GetFloat(soundsVolumeParameter, out float decibels))
					return AudioUtility.DbToPercentage(decibels);

				return 0;
			}
			set => primaryAudioMixer.SetFloat(soundsVolumeParameter, AudioUtility.PercentageToDb(value));
		}
		
		public float GuiSoundsVolume
		{
			get
			{
				if (primaryAudioMixer.GetFloat(guiSoundsVolumeParameter, out float decibels))
					return AudioUtility.DbToPercentage(decibels);

				return 0;
			}
			set => primaryAudioMixer.SetFloat(guiSoundsVolumeParameter, AudioUtility.PercentageToDb(value));
		}
		
		public float DialogueVolume
		{
			get
			{
				if (primaryAudioMixer.GetFloat(dialogueVolumeParameter, out float decibels))
					return AudioUtility.DbToPercentage(decibels);

				return 0;
			}
			set => primaryAudioMixer.SetFloat(dialogueVolumeParameter, AudioUtility.PercentageToDb(value));
		}
		
		public float MusicVolume
		{
			get
			{
				if (primaryAudioMixer.GetFloat(musicVolumeParameter, out float decibels))
					return AudioUtility.DbToPercentage(decibels);

				return 0;
			}
			set => primaryAudioMixer.SetFloat(musicVolumeParameter, AudioUtility.PercentageToDb(value));
		}
		
		private void Awake()
		{
			if (current != null)
				throw new InvalidOperationException("Another AudioManager is already active in the scene. You can only have one AudioManager active at a time and generally shouldn't swap between them.");

			current = this;

			songPlayer = new SongPlayerCore(musicGroup);
			soundPool = new AudioSourcePool(maximumSoundsAtOnce, "Sound Effect Pool");
		}

		private void OnDestroy()
		{
			current = null;
		}

		private void Update()
		{
			songPlayer.Update();
		}

		private void PlaySoundInternal(AudioClip clip, float localVolume, float localPitch, Transform? worldTransform = null)
		{
			PlaySoundInternal(new BasicSoundEffect(clip, localVolume, localPitch), worldTransform);
		}

		private void PlaySoundInternal(ISoundEffect soundEffect, Transform? worldTransform)
		{
			if (!this.soundPool.TryGetAudioSource(out AudioSource? source) || source == null)
				return;

			if (worldTransform != null)
			{
				source.spatialize = true;

				Transform t = source.transform;
				t.position = worldTransform.position;
				t.rotation = worldTransform.rotation;
			}
			else
			{
				source.spatialize = false;
			}
			
			soundEffect.PlayOnAudioSource(source);
		}


		private SongPlayerState PlaySongAsset(SongAsset song, bool loop = false)
		{
			return this.songPlayer.Play(song, loop);
		}

		public static SongPlayerState PlaySong(SongAsset song, bool loop)
		{
			if (Current == null)
				return SongPlayerState.Invalid;

			return Current.PlaySongAsset(song, loop);
		}

		public static UserInterfaceSoundSchemeAsset? GuiSounds
		{
			get
			{
				if (Current == null)
					return null;

				return current.guiSounds;
			}
		}
		
		public static void PlaySound(ISoundEffect soundEffect, Transform? worldTransform = null)
		{
			if (Current == null)
				return;

			Current.PlaySoundInternal(soundEffect, worldTransform);
		}
	}
}