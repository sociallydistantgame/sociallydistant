#nullable enable

using Architecture;
using UnityEngine.Analytics;

namespace UI.CharacterCreator
{
	public class CharacterCreatorState
	{
		public LifepathAsset? Lifepath { get; set; }
		public string? PlayerName { get; set; }
		public string? UserName { get; set; }
		public string? HostName { get; set; }
		public Gender ChosenGender { get; set; }
	}
}