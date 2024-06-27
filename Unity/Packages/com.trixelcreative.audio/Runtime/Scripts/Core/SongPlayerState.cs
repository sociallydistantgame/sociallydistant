using System;

namespace TrixelCreative.TrixelAudio
{
	public class SongPlayerState
	{
		private readonly SongPlayerCore.SongPlayerStateInternal? internalState;

		public float Length => internalState?.Length ?? 0;
		public float Position => internalState?.Position ?? 0;
		public PlaybackState PlaybackState => internalState?.PlaybackState ?? PlaybackState.Stopped;

		public event Action? Stopped; 

		internal SongPlayerState(SongPlayerCore.SongPlayerStateInternal internalState)
		{
			this.internalState = internalState;
			this.internalState.Stopped += HandleStoppedInternally;
		}

		private SongPlayerState()
		{
			this.internalState = null;
		}

		public void Stop()
		{
			if (this.internalState == null)
				return;
			
			if (this.PlaybackState == PlaybackState.Stopped)
				return;

			this.internalState.RequestStop();
		}

		private void HandleStoppedInternally()
		{
			this.Stopped?.Invoke();
		}

		public static SongPlayerState Invalid => new SongPlayerState();
	}
}