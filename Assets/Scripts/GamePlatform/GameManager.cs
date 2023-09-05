#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentManagement;
using Core;
using Core.Serialization.Binary;
using Cysharp.Threading.Tasks;
using GamePlatform.ContentManagement;
using Modding;
using Modules;
using Shell;
using Shell.InfoPanel;
using UI.Shell.InfoPanel;
using UniRx;
using UnityEngine;
using Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GamePlatform
{
	public class GameManager : 
		MonoBehaviour,
		IGameContext
	{
		[SerializeField]
		private WorldManagerHolder worldManager = null!;

		[SerializeField]
		private InfoPanelService infoPanelService = null!;

		private ModuleManager moduleManager;
		private Subject<GameMode> gameMode = new Subject<GameMode>();
		private ContentManager contentManager = new ContentManager();
		private IGameData? currentGameData;
		private PlayerInfo loadedPlayerInfo;
		
		public IObservable<GameMode> GameModeObservable => gameMode;
		
		public bool IsGameActive => false;
		public IContentManager ContentManager => this.contentManager;

		public IInfoPanelService InfoPanelService => this.infoPanelService;

		/// <inheritdoc />
		public IWorldManager WorldManager => this.worldManager.Value!;
		
		private void Awake()
		{
			// Set up ModuleManager with us as the context
			this.moduleManager = new ModuleManager(this);
			
			// Default to loading
			SetGameMode(GameMode.Loading);
			
			// Register mandatory content sources with ContentManager
			contentManager.AddContentSource<LocalGameDataSource>(); // User profiles
		}

		private async UniTaskVoid Start()
		{
			infoPanelService.ClearAllWidgets();
			
			SetGameMode(GameMode.Loading);
			
			// Load mods.
			await moduleManager.LocateAllGameModules();
            
			// Initial ContentManager database rebuild.
			await ContentManager.RefreshContentDatabaseAsync();
			
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
	}
}