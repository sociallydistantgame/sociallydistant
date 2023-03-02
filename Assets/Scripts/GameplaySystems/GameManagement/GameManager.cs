#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core;
using Core.Serialization.Binary;
using Newtonsoft.Json;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace GameplaySystems.GameManagement
{
	public class GameManager
	{
		private readonly string worldFileName = "world.bin";
		private readonly string playerHomeDirectoryName = "home";
		private readonly string npcHomesDirectoryName = "npcs";
		private readonly string paramsFileName = "params.json";
		private WorldManagerHolder world;

		private string? currentGamePath;
		private SaveFileParameters? currentGame;
		
		/// <summary>
		///		Gets a path where all game data must be stored.
		/// </summary>
		public string GameDataDirectory => Application.persistentDataPath;

		/// <summary>
		///		Gets a path where all save files are located.
		/// </summary>
		public string AllSavesDirectory => Path.Combine(GameDataDirectory, "saves");

		public string? CurrentGamePath => currentGamePath;

		public string CurrentPlayerName => currentGame?.playerName ?? "player";
		public string CurrentPlayerHostName => currentGame?.playerComputerName ?? "localhost";

		public event Action GameStarted;
		public event Action GameEnded;
		
		
		public GameManager(WorldManagerHolder world)
		{
			this.world = world;
		}
		
		/// <summary>
		///		Enumerates all save files, returning their parameter data as a collection of
		///		<see cref="SaveFileParameters"/> objects.
		/// </summary>
		/// <returns>An enumerable collection of save files.</returns>
		public IEnumerable<SaveFileParameters> EnumerateAllSaveFiles()
		{
			if (!Directory.Exists(AllSavesDirectory))
			{
				Directory.CreateDirectory(AllSavesDirectory);
				yield break;
			}

			foreach (string saveDirectory in Directory.GetDirectories(AllSavesDirectory))
			{
				string paramsPath = Path.Combine(saveDirectory, paramsFileName);
				if (!File.Exists(paramsPath))
					continue;
				
				string saveId = Path.GetFileName(saveDirectory);

				SaveFileParameters? parameters = null;
				try
				{
					string json = File.ReadAllText(paramsPath);
					parameters = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveFileParameters>(json);
				}
				catch (Exception ex)
				{
					Debug.LogWarning($"Exception occurred while parsing {paramsPath}: {ex}");
					continue;
				}

				if (parameters == null)
					continue;

				parameters.saveId = saveId;
				yield return parameters;
			}
		}

		public void StartGame(SaveFileParameters parameters)
		{
			ThrowIfGameActive();

			if (string.IsNullOrWhiteSpace(parameters.saveId))
				throw new InvalidOperationException("Cannot load the given save file parameters since the save ID is blank.");

			string savePath = Path.Combine(AllSavesDirectory, parameters.saveId);

			string? directoryName = Path.GetDirectoryName(savePath);
			
			if (directoryName != AllSavesDirectory)
				throw new InvalidOperationException($"Loading save files outside of the path \"{AllSavesDirectory}\" is prohibited for security reasons.");

			if (!Directory.Exists(savePath))
				throw new DirectoryNotFoundException(savePath);

			this.currentGame = parameters;
			this.currentGamePath = savePath;

			SaveGameParameters();

			world.Value.WipeWorld();

			string worldFile = Path.Combine(currentGamePath, worldFileName);

			if (!File.Exists(worldFile))
				return;

			using Stream stream = File.OpenRead(worldFile);
			using var reader = new BinaryReader(stream, Encoding.UTF8);
			using var dataReader = new BinaryDataReader(reader);
			
			world.Value.LoadWorld(dataReader);
			GameStarted?.Invoke();
		}

		public void EndCurrentGame()
		{
			SaveCurrentGame();

			world.Value.WipeWorld();
			currentGame = null;
			currentGamePath = null;
			GameEnded?.Invoke();
		}
		
		public void StartNewGame(string saveId, string playerName, string playerHostName)
		{
			ThrowIfGameActive();

			string savePath = Path.Combine(AllSavesDirectory, saveId);
			string? directoryName = Path.GetDirectoryName(savePath);
			
			if (directoryName != AllSavesDirectory)
				throw new InvalidOperationException($"Loading save files outside of the path \"{AllSavesDirectory}\" is prohibited for security reasons.");

			if (Directory.Exists(savePath))
				throw new InvalidOperationException("A save file with the same ID already exists.");

			Directory.CreateDirectory(savePath);
			
			this.currentGamePath = savePath;
			this.currentGame = new SaveFileParameters
			{
				saveId = saveId,
				playerName = playerName,
				playerComputerName = playerHostName,
				creationDate = DateTime.UtcNow,
				lastPlayedDate = DateTime.UtcNow,
				lastPlayedMissionName = string.Empty
			};

			world.Value.WipeWorld();

			SaveCurrentGame();
			GameStarted?.Invoke();
		}

		public void SaveCurrentGame()
		{
			ThrowIfGameNotActive();

			SaveGameParameters();

			string worldFile = Path.Combine(currentGamePath!, worldFileName);
			
			using FileStream stream = File.OpenWrite(worldFile);
			using var writer = new BinaryWriter(stream, Encoding.UTF8);
			using var dataWriter = new BinaryDataWriter(writer);

			world.Value.SaveWorld(dataWriter);
		}
		
		private void ThrowIfGameActive()
		{
			if (currentGame != null)
				throw new InvalidOperationException("A game is already loaded.");
		}
		
		private void ThrowIfGameNotActive()
		{
			if (currentGame == null)
				throw new InvalidOperationException("A game is not loaded.");
		}

		private void SaveGameParameters()
		{
			ThrowIfGameNotActive();

			currentGame!.lastPlayedDate = DateTime.UtcNow;
			
			string paramsFile = Path.Combine(currentGamePath!, paramsFileName);
			string json = JsonConvert.SerializeObject(currentGame!);
			
			File.WriteAllText(paramsFile, json);
		}
	}

	/// <summary>
	///		Represents the contents of the "params.json" file found in a Socially Distant save file.
	/// </summary>
	public class SaveFileParameters
	{
		public string playerName;
		public string playerComputerName;
		public DateTime creationDate;
		public DateTime lastPlayedDate;
		public string lastPlayedMissionName;

		[NonSerialized]
		public string saveId;
	}
}