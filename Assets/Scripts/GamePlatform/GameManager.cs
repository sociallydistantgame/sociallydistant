#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Serialization.Binary;
using Cysharp.Threading.Tasks;
using GamePlatform.ContentManagement;
using UI.Shell.InfoPanel;
using UniRx;
using UnityEngine;
using Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GamePlatform
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField]
		private WorldManagerHolder worldManager = null!;

		[SerializeField]
		private InfoPanelService infoPanelService = null!;
		
		private Subject<GameMode> gameMode = new Subject<GameMode>();
		private ContentManager contentManager = new ContentManager();
		private IGameData? currentGameData;
		private PlayerInfo loadedPlayerInfo;
		
		public IObservable<GameMode> GameModeObservable => gameMode;
		
		public bool IsGameActive => false;
		public ContentManager ContentManager => this.contentManager;

		private void Awake()
		{
			// Default to loading
			SetGameMode(GameMode.Loading);
			
			// Register mandatory content sources with ContentManager
			contentManager.AddContentSource<LocalGameDataSource>(); // User profiles
		}

		private async UniTaskVoid Start()
		{
			infoPanelService.ClearAllWidgets();
			
			SetGameMode(GameMode.Loading);
			
			// Find Restitched save data. If we do, the player gets a little gift from the lead programmer of the game - who.......created this game as well? <3
			FindRestitchedDataAndRegisterRestitchedContent();
			
			// Initial ContentManager database rebuild.
			await ContentManager.RefreshContentDatabase();
			
			// Determine initialization flow.
			InitializationFlow flow = GetInitializationFlow();

			switch (flow)
			{
				case InitializationFlow.MostRecentSave:
				{
					// Find the most recent save file, if any.
					IGameData? saveFile = contentManager.GetContentOfType<IGameData>()
						.OrderByDescending(x => x.PlayerInfo.LastPlayed)
						.FirstOrDefault();
					
					// No save data, go to login screen instead
					if (saveFile == null)
					{
						goto case InitializationFlow.LoginScreen;
						break;
					}

					await StartGame(saveFile);
					break;
				}
				case InitializationFlow.LoginScreen:
				{
					await GoToLoginScreen();
					break;
				}
				case InitializationFlow.DebugWorld:
				{
					this.infoPanelService.CreateStickyInfoWidget(MaterialIcons.BugReport, "Debug world", "The Hypervisor is in debug mode. This OS will be destroyed when you log out. Use the Backtick key (`) to activate the debug UI.");
					
					await StartGame(new DebugGameData());
					break;
				}
			}
		}

		public async Task GoToLoginScreen()
		{
			await EndCurrentGame();

			SetGameMode(GameMode.AtLoginScreen);
		}

		public async Task StartCharacterCreator()
		{
			await EndCurrentGame();
			
			SetGameMode(GameMode.CharacterCreator);
		}
		
		public async Task StartGame(IGameData gameToLoad)
		{
			await EndCurrentGame();
			
			SetGameMode(GameMode.Loading);

			this.loadedPlayerInfo = gameToLoad.PlayerInfo;
			this.loadedPlayerInfo.LastPlayed = DateTime.UtcNow;

			await gameToLoad.UpdatePlayerInfo(this.loadedPlayerInfo);

			// Load world data
			if (worldManager.Value != null)
			{
				using var memory = new MemoryStream();
				bool result = await gameToLoad.ExtractWorldData(memory);
				if (!result)
				{
					// Couldn't extract a world, fail and bail.
					await GoToLoginScreen();
					return;
				}

				memory.Seek(0, SeekOrigin.Begin);

				this.worldManager.Value.WipeWorld();
				
				if (memory.Length > 0)
				{
					using var binaryReader = new BinaryReader(memory, Encoding.UTF8);
					using var worldReader = new BinaryDataReader(binaryReader);

					this.worldManager.Value.LoadWorld(worldReader);
				}
			}

			this.currentGameData = gameToLoad;

			SetGameMode(GameMode.LockScreen);
		}
		
		public async Task SaveCurrentGame()
		{
			if (currentGameData == null)
				return;

			await currentGameData.UpdatePlayerInfo(loadedPlayerInfo);
		}

		public async Task EndCurrentGame()
		{
			await SaveCurrentGame();

			this.currentGameData = null;
		}

		private void SetGameMode(GameMode newGameMode)
		{
			this.gameMode.OnNext(newGameMode);
		}

		private InitializationFlow GetInitializationFlow()
		{
#if UNITY_EDITOR
			int initializationFlowId =  EditorPrefs.GetInt(InitializationFlowEditorPreference, (int) InitializationFlow.MostRecentSave);
			return (InitializationFlow) initializationFlowId;
#else
			return InitializationFlow.MostRecentSave;
#endif
		}

		#if UNITY_EDITOR
		public static readonly string InitializationFlowEditorPreference = "dev.acidiclight.sociallydistant.initializationFlow";
		#endif
		
		public enum InitializationFlow
		{
			LoginScreen,
			MostRecentSave,
			DebugWorld
		}

		private void FindRestitchedDataAndRegisterRestitchedContent()
		{
			const string restitchedDetectionFlag = "dev.acidiclight.sociallydistant.specialContentFlags.foundRestitchedGameData";
			
			// Check the flag in PlayerPrefs to see if we've already located Restitched on this device before
			bool hasFoundRestitchedAlready = PlayerPrefs.GetInt(restitchedDetectionFlag, 0) == 1;

			if (!hasFoundRestitchedAlready)
			{
				// Restitched is a Unity game, and will thus store data in the Applicatioon.persistentDataPath folder just like us.
				// The trick is, while  we have our own dev and product name set, Restitched has a different one... so the path will be different.
				// We can walk up the directory twice to get rid of our dev/product name, then combine that base directory with the Trixel dev/product name.
				const string trixelDevName = "Trixel Creative";
				const string trixelProductName = "Restitched";

				string ourData = Application.persistentDataPath;

				string acidicData = Path.GetDirectoryName(ourData)!;
				string baseDirectory = Path.GetDirectoryName(acidicData)!;

				string restitchedData = Path.Combine(baseDirectory, trixelDevName, trixelProductName);

				// Check if the directory exists.
				if (!Directory.Exists(restitchedData))
					return;

				// Make sure the user has actually done something in the game.
				// There are two ways we can approach this.
				// 1. Check for a Stuffy. These are located in <restitchedData>/bears, and have a .stuffy extension. They're files.
				// 2. Check for a Build Mode level. These are stored in <restitchedPath>/levels, and end in .tclevel.

				string bearsPath = Path.Combine(restitchedData, "bears");
				string levelsPath = Path.Combine(restitchedData, "levels");

				// Check both directories to see if they exist.
				bool bearsExists = Directory.Exists(bearsPath);
				bool levelsExists = Directory.Exists(levelsPath);

				var stuffyFound = false;
				var levelFound = false;

				if (bearsExists)
				{
					string[] files = Directory.GetFiles(bearsPath);

					if (files.Any(x => x.ToLower().EndsWith(".stuffy")))
						stuffyFound = true;
				}

				if (levelsExists)
				{
					string[] files = Directory.GetFiles(levelsPath);

					if (files.Any(x => x.ToLower().EndsWith(".tclevel")))
						levelFound = true;
				}

				// Check if at least a stuffy or level was found.
				if (!(levelFound || stuffyFound))
					return;
				
				// Set the flag. Only in builds though.
#if !UNITY_EDITOR
				PlayerPrefs.SetInt(restitchedDetectionFlag, 1);
#endif
				
				// Display a message to the user in their info panel when they've logged in.
				infoPanelService.CreateCloseableInfoWidget(MaterialIcons.Brush, "New content unlocked!", "The Hypervisor has detected saved game data on your host computer from Restitched. Some Restitched-themed content has been added to your OS. Go to System Settings -> Customization to see what's new.");
			}

            // Register Restitched content with ContentManager.
            this.contentManager.AddContentSource<RestitchedContentSource>();
		}
	}
}