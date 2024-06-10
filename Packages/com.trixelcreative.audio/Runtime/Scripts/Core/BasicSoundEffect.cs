using UnityEngine;

namespace TrixelCreative.TrixelAudio.Core
{
	public sealed class BasicSoundEffect : ISoundEffect
	{
		private readonly AudioClip clip;
		private readonly float volume;
		private readonly float pitch;

		public BasicSoundEffect(AudioClip clip, float volume, float pitch)
		{
			this.clip = clip;
			this.volume = volume;
			this.pitch = pitch;
		}

		/// <inheritdoc />
		public void PlayOnAudioSource(AudioSource audioSource)
		{
			audioSource.volume = this.volume;
			audioSource.pitch = this.pitch;

			audioSource.PlayOneShot(this.clip);
		}
	}
}