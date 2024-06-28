using UnityEngine;

namespace TrixelCreative.TrixelAudio.Data
{
	[CreateAssetMenu(menuName = "TrixelAudio/Song Asset")]
	public class SongAsset : ScriptableObject
	{
		
		private string songTitle = string.Empty;
		
		
		private string artist = string.Empty;

		
		private AudioClip audioClip = null!;

		public string Title => songTitle;
		public string Artist => artist;

		public float Length => this.audioClip != null ? this.audioClip.length : 0f;
		
		internal void Setup(AudioSource audioSource)
		{
			if (this.audioClip == null)
			{
				Debug.LogWarning($"[TrixelAudio] Song \"{name}\" cannot be played because it has no audio clip assigned.");
				return;
			}

			audioSource.clip = this.audioClip;
			audioSource.Play();
		}
	}
}