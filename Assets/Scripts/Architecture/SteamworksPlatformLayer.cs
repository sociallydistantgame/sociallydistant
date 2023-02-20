#nullable enable
using System;

namespace Architecture
{
	public class SteamworksPlatformLayer : IGamePlatformLayer
	{
		/// <inheritdoc />
		public string GetPlayerName()
		{
			return "Desire2Leave";
		}
	}
}