using System.Collections.Generic;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Data
{
	[CreateAssetMenu(menuName = "TrixelAudio/Playlist")]
	public class PlaylistAsset : ScriptableObject
	{
		
		private string playlistName = string.Empty;

		
		private List<SongAsset> songs = new List<SongAsset>();

		public string Name => playlistName;
		public IReadOnlyList<SongAsset> Songs => songs;
	}
}