#nullable enable
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace GamePlatform
{
	public class LocalGameData : IGameData
	{
		public static string BaseDirectory = Path.Combine(Application.persistentDataPath, "users");

		public static readonly string WorldFileName = "world.bin";
		public static readonly string PlayerInfoFileName = "playerinfo.xml";
		
		/// <inheritdoc />
		public PlayerInfo PlayerInfo { get; private set; }

		private readonly string directory;
		
		private LocalGameData(string directory)
		{
			this.directory = directory;
		}
		
		/// <inheritdoc />
		public Task<Texture2D> GetPlayerAvatar()
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public Task<Texture2D> GetPlayerCoverPhoto()
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<bool> ExtractWorldData(Stream destinationStream)
		{
			if (!destinationStream.CanWrite)
				return false;
			
			if (destinationStream.CanSeek)
				destinationStream.Seek(0, SeekOrigin.Begin);
			
			destinationStream.SetLength(0);

			string worldPath = Path.Combine(directory, WorldFileName);

			if (!File.Exists(worldPath))
				return false;
			
			await using FileStream fs = File.OpenRead(worldPath);

			await fs.CopyToAsync(destinationStream);

			return true;
		}

		/// <inheritdoc />
		public async Task UpdatePlayerInfo(PlayerInfo newPlayerInfo)
		{
			this.PlayerInfo = newPlayerInfo;

			string playerInfoPath = Path.Combine(directory, PlayerInfoFileName);
			await using FileStream fs = File.Open(playerInfoPath, FileMode.OpenOrCreate);
			fs.SetLength(0);

			using var writer = new StreamWriter(fs);

			string xml = newPlayerInfo.ToXML();

			await writer.WriteAsync(xml);
		}

		private async Task<bool> LoadPlayerData()
		{
			string playerInfoPath = Path.Combine(directory, PlayerInfoFileName);
			string worldPath = Path.Combine(directory, WorldFileName);

			if (!File.Exists(playerInfoPath))
				return false;
			
			if (!File.Exists(worldPath))
				return false;

			await using FileStream fileStream = File.OpenRead(playerInfoPath);
			using var reader = new StreamReader(fileStream);

			string xml = await reader.ReadToEndAsync();

			if (!PlayerInfo.TryGetFromXML(xml, out PlayerInfo info))
				return false;

			this.PlayerInfo = info;
			return true;
		}
		
		public static async Task<LocalGameData?> TryLoadFromDirectory(string directory)
		{
			if (!Directory.Exists(directory))
				return null;

			var data = new LocalGameData(directory);

			if (!await data.LoadPlayerData())
				return null;

			return data;
		}
	}
}