#nullable enable
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core;

namespace SociallyDistant.Core.Modules
{
	/// <summary>
	///		Interface for an object that can be loaded as a Socially Distant game session.
	/// </summary>
	public interface IGameData : IGameContent
	{
		PlayerInfo PlayerInfo { get; }

		string? LocalFilePath { get; }
		
		Task<Texture2D> GetPlayerAvatar();
		Task<Texture2D> GetPlayerCoverPhoto();

		Task<bool> ExtractWorldData(Stream destinationStream);

		Task UpdatePlayerInfo(PlayerInfo newPlayerInfo);
		Task SaveWorld(IWorldManager world);
	}
}