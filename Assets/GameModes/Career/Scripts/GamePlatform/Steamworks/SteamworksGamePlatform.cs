#nullable enable

using GamePlatform;
using Steamworks;

namespace GameModes.Career.Scripts.GamePlatform.Steamworks
{
	public class SteamworksGamePlatform : IGamePlatform
	{
		/// <inheritdoc />
		public string QueryPlayerName()
		{
			return SteamClient.Name;
		}
	}
}