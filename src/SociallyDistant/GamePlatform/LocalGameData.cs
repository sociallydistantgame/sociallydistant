#nullable enable
using System.Net.Mime;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Serialization.Binary;
using SociallyDistant.Core.WorldData;

namespace SociallyDistant.GamePlatform
{
	public class LocalGameData : IGameData
	{
		public static string BaseDirectory = Path.Combine(SociallyDistantGame.GameDataPath, "users");

		public static readonly string WorldFileName = "world.bin";
		public static readonly string PlayerInfoFileName = "playerinfo.xml";
		
		/// <inheritdoc />
		public PlayerInfo PlayerInfo { get; private set; }

		/// <inheritdoc />
		public string? LocalFilePath => this.directory;

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

			await using var writer = new StreamWriter(fs);
			string xml = newPlayerInfo.ToXML();

			await writer.WriteAsync(xml);
		}

		private bool LoadPlayerData()
		{
			string playerInfoPath = Path.Combine(directory, PlayerInfoFileName);
			string worldPath = Path.Combine(directory, WorldFileName);

			if (!File.Exists(playerInfoPath))
				return false;
			
			if (!File.Exists(worldPath))
				return false;

			using FileStream fileStream = File.OpenRead(playerInfoPath);
			using var reader = new StreamReader(fileStream);

			string xml = reader.ReadToEnd();

			if (!PlayerInfo.TryGetFromXML(xml, out PlayerInfo info))
				return false;

			this.PlayerInfo = info;
			return true;
		}
		
		public static LocalGameData TryLoadFromDirectory(string directory)
		{
			if (!Directory.Exists(directory))
				return null;

			var data = new LocalGameData(directory);

			if (!data.LoadPlayerData())
				return null;

			return data;
		}

		/// <inheritdoc />
		public async Task SaveWorld(IWorldManager worldManager)
		{
			// Serialize the world to RAM first, because that'll be faster than writing to disk.
			// Serializing the world is a blocking operation.
			using var memory = new MemoryStream();
			await using (var binaryWriter = new BinaryWriter(memory, Encoding.UTF8, true))
			{
				var binarySerializer = new BinaryDataWriter(binaryWriter);
				
				// we can do this on another thread because saving the world should not modify it.
				await Task.Run(() =>
				{
					worldManager.SaveWorld(binarySerializer);
				});
			}

			memory.Seek(0, SeekOrigin.Begin);
			
			// Now we can open the world file for writing
			string worldFilePath = Path.Combine(directory, WorldFileName);

			await using Stream fileStream = File.OpenWrite(worldFilePath);
			fileStream.SetLength(0);

			await memory.CopyToAsync(fileStream);
		}
		
		public static async Task<LocalGameData> CreateNewGame(PlayerInfo playerInfo, World world)
		{
			var gameIndex = 0;

			string baseDir = BaseDirectory;

			string fileName = string.Empty;
			string fullPath = string.Empty;

			do
			{
				fileName = $"SARS-CoV-{gameIndex + 2}";
				fullPath = Path.Combine(baseDir, fileName);
				gameIndex++;
			} while (Directory.Exists(fullPath));

			Directory.CreateDirectory(fullPath);

			var gameData = new LocalGameData(fullPath);

			await gameData.UpdatePlayerInfo(playerInfo);

			await WorldManager.Instance.RestoreWorld(world);
			await gameData.SaveWorld(WorldManager.Instance);

			return gameData;
		}
	}
}