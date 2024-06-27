using System.Collections.Generic;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Data
{
	[CreateAssetMenu(menuName = "TrixelAudio/Playlist")]
	public class PlaylistAsset : ScriptableObject
	{
		[SerializeField]
		private string playlistName = string.Empty;

		[SerializeField]
		private List<SongAsset> songs = new List<SongAsset>();

		public string Name => playlistName;
		public IReadOnlyList<SongAsset> Songs => songs;
	}
}