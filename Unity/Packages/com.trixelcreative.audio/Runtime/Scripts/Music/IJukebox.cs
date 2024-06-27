using TrixelCreative.TrixelAudio.Data;

namespace TrixelCreative.TrixelAudio.Music
{
	public interface IJukebox
	{
		SongAsset? Song { get; }
		PlaybackState PlaybackState { get; }
		
		void Play();
		void Stop();
	}
}