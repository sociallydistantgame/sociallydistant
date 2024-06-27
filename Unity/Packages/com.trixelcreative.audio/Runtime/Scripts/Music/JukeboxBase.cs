#nullable enable

using TrixelCreative.TrixelAudio.Core;
using TrixelCreative.TrixelAudio.Data;
using TrixelCreative.TrixelAudio.Utility;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Music
{
	public abstract class JukeboxBase : MonoBehaviour, IJukebox
	{
		private SongPlayerState? currentSongState;

		[SerializeField]
		private SongAsset? song;

		[SerializeField]
		private bool loop = false;

		public PlaybackState PlaybackState
		{
			get
			{
				if (this.currentSongState == null)
					return PlaybackState.Stopped;

				return this.currentSongState.PlaybackState;
			}
		}
		
		public SongAsset? Song => this.song;
		
		private void Awake()
		{
			OnAwake();
		}

		protected virtual void OnAwake() { }
		
		public void Play()
		{
			if (song != null)
			{
				this.currentSongState = AudioManager.PlaySong(this.song, this.loop);
				this.currentSongState.Stopped += this.HandleSongStopped;
			}
		}

		public void Stop()
		{
			if (currentSongState != null)
			{
				// This will fire off a chain of events that ultimately results in currentSongState being nulled out
				currentSongState.Stop();
			}
		}
		
		public void SetSong(SongAsset? newSong)
		{
			if (this.song == newSong)
				return;
			
			// If we're not already playing a song, just update the song. Nothing else to do.
			if (currentSongState == null)
			{
				this.song = newSong;
				return;
			}
			
			// Stop the current song
			Stop();
			
			// Update the song
			this.song = newSong;
		}

		private void HandleSongStopped()
		{
			if (currentSongState != null)
			{
				currentSongState.Stopped -= HandleSongStopped;
				currentSongState = null;
			}
		}
	}
}