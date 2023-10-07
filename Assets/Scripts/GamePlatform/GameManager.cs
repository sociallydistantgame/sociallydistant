#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentManagement;
using Core;
using Core.Config;
using Core.Serialization.Binary;
using Core.WorldData.Data;
using Cysharp.Threading.Tasks;
using GamePlatform.ContentManagement;
using GameplaySystems.Social;
using log4net.ObjectRenderer;
using Modding;
using Modules;
using OS;
using Player;
using Shell;
using Shell.InfoPanel;
using Social;
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
		private PlayerInstanceHolder playerInstance = null!;
		
		[SerializeField]
		private WorldManagerHolder worldManager = null!;

		[SerializeField]
		private InfoPanelService infoPanelService = null!;

		[SerializeField]
		private SocialServiceHolder socialHolder = null!;

		private GameMode currentGameMode;
		private SettingsManager settingsManager;
		private ModuleManager moduleManager;
		private Subject<GameMode> gameMode = new Subject<GameMode>();
		private ContentManager contentManager = new ContentManager();
		private IGameData? currentGameData;
		private PlayerInfo loadedPlayerInfo;

		public IObservable<GameMode> GameModeObservable { get; private set; }

		public bool IsGameActive => false;
		public IContentManager ContentManager => this.contentManager;

		public IInfoPanelService InfoPanelService => this.infoPanelService;

		/// <inheritdoc />
		public ISocialService SocialService => socialHolder.Value!;

		/// <inheritdoc />
		public IKernel Kernel => playerInstance;
		
		/// <inheritdoc />
		public IShellContext Shell => playerInstance.Value.UiManager;
        
		/// <inheritdoc />
		public GameMode CurrentGameMode => this.currentGameMode;
		
		/// <inheritdoc />
		public IWorldManager WorldManager => this.worldManager.Value!;

		/// <inheritdoc />
		public ISettingsManager SettingsManager => settingsManager;
		
		private void Awake()
		{
			GameModeObservable = Observable.Create<GameMode>(observer =>
			{
				observer.OnNext(currentGameMode);
				return gameMode.Subscribe(observer);
			});
			
			// Default to loading
			SetGameMode(GameMode.Loading);
			
			// Initialize the Registry.
			settingsManager  = new SettingsManager();
			
			// Set up ModuleManager with us as the context
			this.moduleManager = new ModuleManager(this);
            
			// Register mandatory content sources with ContentManager
			contentManager.AddContentSource<LocalGameDataSource>(); // User profiles
		}

		private void OnDestroy()
		{
			settingsManager.Dispose();
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
				
				// Create player profile data if it's missing
				WorldPlayerData playerData = this.worldManager.Value.World.PlayerData.Value;

				var modify = false;
				WorldProfileData profile = default;
				if (worldManager.Value.World.Profiles.ContainsId(playerData.PlayerProfile))
				{
					profile = worldManager.Value.World.Profiles[playerData.PlayerProfile];
					modify = true;
				}
				else
				{
					profile.InstanceId = worldManager.Value.GetNextObjectId();
					playerData.PlayerProfile = profile.InstanceId;
				}
				
				// Sync the profile data with the save's metadata
				// We only sync the gender and full names.
				profile.Gender = this.loadedPlayerInfo.PlayerGender;
				profile.SocialName = this.loadedPlayerInfo.Name;
				profile.ChatName = this.loadedPlayerInfo.Name;
				profile.MailName = this.loadedPlayerInfo.Name;

				if (modify)
				{
					worldManager.Value.World.Profiles.Modify(profile);
				}
				else
				{
					worldManager.Value.World.Profiles.Add(profile);
					worldManager.Value.World.PlayerData.Value = playerData;
				}
			}

			this.currentGameData = gameToLoad;
			
			
			
			SetGameMode(GameMode.LockScreen);
		}

		public async Task QuitVM()
		{
			// Put the game into loading state
			SetGameMode(GameMode.Loading);
			
			// Save and end the current game
			await EndCurrentGame();

			PlatformHelper.QuitToDesktop();
		}
		
		public async Task SaveCurrentGame()
		{
			if (currentGameData == null)
				return;

			await currentGameData.UpdatePlayerInfo(loadedPlayerInfo);
			await currentGameData.SaveWorld(worldManager.Value);
		}

		public async Task EndCurrentGame()
		{
			await SaveCurrentGame();

			this.currentGameData = null;
		}

		private void SetGameMode(GameMode newGameMode)
		{
			currentGameMode = newGameMode;
			this.gameMode.OnNext(currentGameMode);
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