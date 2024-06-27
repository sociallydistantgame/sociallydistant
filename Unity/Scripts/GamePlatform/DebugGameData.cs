#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using Core;
using UnityEngine;

namespace GamePlatform
{
	public class DebugGameData : IGameData
	{
		/// <inheritdoc />
		public PlayerInfo PlayerInfo { get; private set; }

		/// <inheritdoc />
		public string? LocalFilePath => null;

		/// <inheritdoc />
		public Task<Texture2D> GetPlayerAvatar()
		{
			return Task.FromResult<Texture2D>(null);
		}

		/// <inheritdoc />
		public Task<Texture2D> GetPlayerCoverPhoto()
		{
			return Task.FromResult<Texture2D>(null);
		}

		/// <inheritdoc />
		public Task<bool> ExtractWorldData(Stream destinationStream)
		{
			return Task.FromResult(true);
		}

		/// <inheritdoc />
		public Task UpdatePlayerInfo(PlayerInfo newPlayerInfo)
		{
			this.PlayerInfo = newPlayerInfo;
			return Task.FromResult(true);
		}

		/// <inheritdoc />
		public Task SaveWorld(IWorldManager world)
		{
			Debug.Log("If you were in an actual game, we'd be saving the world to disk right now. But this is the debug world, so we're not.");
			return Task.CompletedTask;
		}
	}
}