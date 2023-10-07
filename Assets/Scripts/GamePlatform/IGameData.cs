#nullable enable
using System.IO;
using System.Threading.Tasks;
using ContentManagement;
using Core;
using GamePlatform.ContentManagement;
using UnityEngine;

namespace GamePlatform
{
	/// <summary>
	///		Interface for an object that can be loaded as a Socially Distant game session.
	/// </summary>
	public interface IGameData : IGameContent
	{
		PlayerInfo PlayerInfo { get; }

		Task<Texture2D> GetPlayerAvatar();
		Task<Texture2D> GetPlayerCoverPhoto();

		Task<bool> ExtractWorldData(Stream destinationStream);

		Task UpdatePlayerInfo(PlayerInfo newPlayerInfo);
		Task SaveWorld(WorldManager world);
	}
}