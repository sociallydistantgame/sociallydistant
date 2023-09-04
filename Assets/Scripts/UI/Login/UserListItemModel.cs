#nullable enable
using GamePlatform;

namespace UI.Login
{
	public class UserListItemModel
	{
		public string Name { get; set; } = string.Empty;
		public string Comments { get; set; } = string.Empty;
		
		public IGameData? GameData { get; set; }
	}
}