﻿namespace SociallyDistant.GamePlatform
{
	public class OfflineGamePlatform : IGamePlatform
	{
		/// <inheritdoc />
		public string QueryPlayerName()
		{
			return "Player";
		}
	}
}