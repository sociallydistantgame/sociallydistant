#nullable enable
using System;

namespace GameplaySystems.GameManagement
{
	/// <summary>
	///		Represents the contents of the "params.json" file found in a Socially Distant save file.
	/// </summary>
	[Serializable]
	public class SaveFileParameters
	{
		public string? playerName;
		public string? playerComputerName;
		public DateTime creationDate;
		public DateTime lastPlayedDate;
		public string? lastPlayedMissionName;

		[NonSerialized]
		public string? saveId;
	}
}