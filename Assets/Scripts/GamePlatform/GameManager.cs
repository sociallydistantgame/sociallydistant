#nullable enable

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Architecture;
using ContentManagement;
using Core;
using Core.Config;
using Core.Scripting;
using Core.Serialization.Binary;
using Core.WorldData;
using Core.WorldData.Data;
using Cysharp.Threading.Tasks;
using GamePlatform.ContentManagement;
using GameplaySystems.Social;
using Modding;
using Modules;
using OS;
using Player;
using Shell;
using Shell.InfoPanel;
using Social;
using UI.PlayerUI;
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
		private ShellScriptAsset gameInitializationScript = null!;
		
		[SerializeField]
		private PlayerInstanceHolder playerInstance = null!;
		
		[SerializeField]
		private InfoPanelService infoPanelService = null!;

		[SerializeField]
		private SocialServiceHolder socialHolder = null!;

		[SerializeField]
		private MainToolGroup terminal = null!;

		private static readonly Singleton<GameManager> singleton = new();
		private static readonly ConcurrentQueue<Action> staticActionQueue = new ConcurrentQueue<Action>();
		
		private readonly UnityTextConsole unityConsole = new UnityTextConsole();
		private bool areModulesLoaded = false;
		private WorldManager worldManager;
		private IScriptSystem scriptSystem;
		private TabbedToolCollection availableTools;
		private GameMode currentGameMode;
		private SettingsManager settingsManager;
		private ModuleManager moduleManager;
		private Subject<GameMode> gameMode = new Subject<GameMode>();
		private ContentManager contentManager = null!;
		private IGameData? currentGameData;
		private PlayerInfo loadedPlayerInfo;
		private Subject<bool> panicSubject = new Subject<bool>();
		private bool panicking;
		private IUriManager uriManager;
		private Subject<PlayerInfo> playerInfoSubject = new Subject<PlayerInfo>();

		public PlayerInstance PlayerInstance => this.playerInstance.Value;
		public IObservable<PlayerInfo> PlayerInfoObservable => playerInfoSubject;
		public IObservable<GameMode> GameModeObservable { get; private set; }
		public IObservable<bool> PanicObservable { get; private set; }

		public bool IsGameActive => false;
		public IContentManager ContentManager => this.contentManager;

		public IInfoPanelService InfoPanelService => this.infoPanelService;

		private event Action? ModulesLoaded;
		
		/// <inheritdoc />
		public TabbedToolCollection AvailableTools => availableTools;
        
		/// <inheritdoc />
		public ISocialService SocialService => socialHolder.Value!;

		/// <inheritdoc />
		public IKernel Kernel => playerInstance;
		
		/// <inheritdoc />
		public IShellContext Shell => playerInstance.Value.UiManager;
        
		/// <inheritdoc />
		public GameMode CurrentGameMode => this.currentGameMode;

		/// <inheritdoc />
		public IWorldManager WorldManager => worldManager;

		/// <inheritdoc />
		public ISettingsManager SettingsManager => settingsManager;

		/// <inheritdoc />
		public IScriptSystem ScriptSystem => scriptSystem;

		/// <inheritdoc />
		public IUriManager UriManager => uriManager;
		
		/// <inheritdoc />
		public string? CurrentSaveDataDirectory => this.currentGameData?.LocalFilePath;
		
		private void Awake()
		{
			worldManager = new WorldManager(this);
			
			singleton.SetInstance(this);
			
			contentManager = new ContentManager(this);
			scriptSystem = new ScriptSystem(this);
			availableTools = new TabbedToolCollection(this);
			uriManager = new UriManager(this);
			
			GameModeObservable = Observable.Create<GameMode>(observer =>
			{
				observer.OnNext(currentGameMode);
				return gameMode.Subscribe(observer);
			});
			
			PanicObservable = Observable.Create<bool>(observer =>
			{
				observer.OnNext(panicking);
				return panicSubject.Subscribe(observer);
			});
            
			// Default to loading
			SetGameMode(GameMode.Loading);
			
			// Initialize the Registry.
			settingsManager  = new SettingsManager();
			
			// Set up ModuleManager with us as the context
			this.moduleManager = new ModuleManager(this);
            
			// Register mandatory content sources with ContentManager
			contentManager.AddContentGenerator(new LocalGameDataSource()); // User profiles
		}
		
		private void OnDestroy()
		{
			settingsManager.Dispose();
			singleton.SetInstance(null);
		}

		private async void Start()
		{
			scriptSystem.RegisterHookListener(CommonScriptHooks.AfterContentReload, new UpdateAvailableToolsHook(contentManager, availableTools, terminal));
			
			infoPanelService.ClearAllWidgets();

			this.SettingsManager.Load();
			SetGameMode(GameMode.Booting);
		}
		
		public async Task GoToLoginScreen()
		{
			await EndCurrentGame(true);

			SetGameMode(GameMode.AtLoginScreen);
		}

		public async Task StartCharacterCreator()
		{
			await EndCurrentGame(true);
			
			SetGameMode(GameMode.CharacterCreator);
		}
		
		public async Task StartGame(IGameData gameToLoad)
		{
			await EndCurrentGame(true);
			
			SetGameMode(GameMode.Loading);

			try
			{
				this.loadedPlayerInfo = gameToLoad.PlayerInfo;
				this.loadedPlayerInfo.LastPlayed = DateTime.UtcNow;

				await gameToLoad.UpdatePlayerInfo(this.loadedPlayerInfo);

				using var memory = new MemoryStream();
				bool result = await gameToLoad.ExtractWorldData(memory);
				if (!result)
				{
					// Couldn't extract a world, fail and bail.
					await GoToLoginScreen();
					return;
				}

				memory.Seek(0, SeekOrigin.Begin);

				var world = Core.WorldManager.Instance;

				await Task.Run(() =>
				{
					world.WipeWorld();

					if (memory.Length > 0)
					{
						using var binaryReader = new BinaryReader(memory, Encoding.UTF8);
						using var worldReader = new BinaryDataReader(binaryReader);

						world.LoadWorld(worldReader);
					}
				});

				while (staticActionQueue.Count > 0)
					await Task.Yield();
				
				// Create player profile data if it's missing
				WorldPlayerData playerData = world.World.PlayerData.Value;

				WorldProfileData profile = default;
				if (!world.World.Profiles.ContainsId(playerData.PlayerProfile))
				{
					profile.InstanceId = world.GetNextObjectId();
					playerData.PlayerProfile = profile.InstanceId;

					// Sync the profile data with the save's metadata
					// We only sync the gender and full names.
					profile.Gender = this.loadedPlayerInfo.PlayerGender;
					profile.ChatName = this.loadedPlayerInfo.Name;

					world.World.Profiles.Add(profile);
					world.World.PlayerData.Value = playerData;
				}
				
				this.currentGameData = gameToLoad;
				
				await gameInitializationScript.ExecuteAsync(unityConsole);
			}
			catch (Exception ex)
			{
				this.playerInstance.Value.UiManager.ShowGameLoadError(ex);

				await EndCurrentGame(false);
				await GoToLoginScreen();
				return;
			}

			playerInfoSubject.OnNext(this.loadedPlayerInfo);
			SetGameMode(GameMode.OnDesktop);
		}

		private void Update()
		{
			if (staticActionQueue.TryDequeue(out Action action))
				action();

			worldManager.UpdateWorldClock();
		}

		public async Task QuitVM()
		{
			// Put the game into loading state
			SetGameMode(GameMode.Loading);
			
			// Save and end the current game
			await EndCurrentGame(true);

			PlatformHelper.QuitToDesktop();
		}
		
		/// <inheritdoc />
		public async Task SaveCurrentGame(bool silent)
		{
			if (currentGameData == null)
				return;
			
			await currentGameData.UpdatePlayerInfo(loadedPlayerInfo);
			await currentGameData.SaveWorld(WorldManager);
			
			if (silent)
				return;
			
			await gameInitializationScript.ExecuteAsync(unityConsole);
		}

		/// <inheritdoc />
		public async Task EndCurrentGame(bool save)
		{
			if (save)
				await SaveCurrentGame(true);

			// we do this to disable the simulation
			SetGameMode(GameMode.Loading);
			
			this.currentGameData = null;
			this.loadedPlayerInfo = default;
			this.playerInfoSubject.OnNext(this.loadedPlayerInfo);

			Core.WorldManager.Instance.WipeWorld();
		}

		/// <inheritdoc />
		public bool IsDebugWorld => currentGameData is DebugGameData;

		/// <inheritdoc />
		public async void SetPlayerHostname(string hostname)
		{
			this.loadedPlayerInfo.HostName = hostname;
			this.playerInfoSubject.OnNext(this.loadedPlayerInfo);

			await this.SaveCurrentGame(true);
		}

		private void SetGameMode(GameMode newGameMode)
		{
			currentGameMode = newGameMode;
			this.gameMode.OnNext(currentGameMode);
		}

		public async Task DoUserInitialization()
		{
			InitializationFlow flow = GetInitializationFlow();

			IGameData? saveToLoad = null;

			if (flow == InitializationFlow.DebugWorld)
			{
				saveToLoad = new DebugGameData();
			}
			else if (flow == InitializationFlow.MostRecentSave)
			{
				saveToLoad = ContentManager.GetContentOfType<IGameData>()
					.OrderByDescending(x => x.PlayerInfo.LastPlayed)
					.FirstOrDefault();
			}

			if (saveToLoad != null)
			{
				await StartGame(saveToLoad);
				return;
			}

			await GoToLoginScreen();
		}
		
		public InitializationFlow GetInitializationFlow()
		{
			var modSettings = new ModdingSettings(settingsManager);
			var uiSettings = new UiSettings(settingsManager);

			if (modSettings.ModDebugMode)
				return InitializationFlow.DebugWorld;

			return uiSettings.PreferredInitializationFlow;
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

		public async Task WaitForModulesToLoad(bool doReload = false)
		{
			if (areModulesLoaded && !doReload)
				return;

			areModulesLoaded = false;

			await moduleManager.LocateAllGameModules();

			areModulesLoaded = true;
		}

		public static void ScheduleAction(Action action)
		{
			staticActionQueue.Enqueue(action);
		}

		internal sealed class UpdateAvailableToolsHook : IHookListener
		{
			private readonly IContentManager contentManager;
			private readonly TabbedToolCollection collection;
			private readonly MainToolGroup terminal;

			public UpdateAvailableToolsHook(IContentManager contentManager, TabbedToolCollection collection, MainToolGroup terminal)
			{
				this.contentManager = contentManager;
				this.collection = collection;
				this.terminal = terminal;
			}
			
			/// <inheritdoc />
			public async Task ReceiveHookAsync(IGameContext game)
			{
				collection.Clear();
				collection.Add(terminal);

				foreach (ITabbedToolDefinition group in contentManager.GetContentOfType<ITabbedToolDefinition>())
				{
					collection.Add(group);
				}
				
				await game.ScriptSystem.RunHookAsync(CommonScriptHooks.BeforeUpdateShellTools);
			}
		}

		public static GameManager Instance => singleton.MustGetInstance();
	}
}