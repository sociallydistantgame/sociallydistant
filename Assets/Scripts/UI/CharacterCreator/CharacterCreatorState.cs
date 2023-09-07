using UnityEngine.Analytics;

namespace UI.CharacterCreator
{
	public class CharacterCreatorState
	{
		public string? PlayerName { get; set; }
		public string? UserName { get; set; }
		public string? HostName { get; set; }
		public Gender ChosenGender { get; set; }
	}
}