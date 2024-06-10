using System;
using TrixelCreative.TrixelAudio.Core;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Data
{
    [CreateAssetMenu(menuName = "TrixelAudio/Sound Effect")]
    public class SoundEffectAsset : 
        ScriptableObject,
        ISoundEffect
    {
        [SerializeField]
        private string soundEffectName = string.Empty;

        [SerializeField]
        private AudioClip? audioClip;

        [SerializeField]
        [Range(0, 1)]
        private float volume = 1;

        [SerializeField]
        [Range(0, 2)]
        private float pitch = 1;

        public string Name => soundEffectName;

        public void PlayOnAudioSource(AudioSource audioSource)
        {
            if (this.audioClip == null)
            {
                Debug.LogWarning($"[TrixelAudio] Sound Effect \"{Name}\" was played but it doesn't have an AudioClip assigned.");
                return;
            }

            if (audioSource == null)
            {
                Debug.LogError($"[TrixelAudio] Sound effect \"{Name}\" played on a null or destroyed AudioSource");
                return;
            }

            audioSource.volume = this.volume;
            audioSource.pitch = this.pitch;

            audioSource.PlayOneShot(this.audioClip);
        }
    }
}