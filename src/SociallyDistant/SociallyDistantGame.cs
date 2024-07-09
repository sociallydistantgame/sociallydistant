using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using AcidicGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using SociallyDistant.Architecture;
using SociallyDistant.Core;
using SociallyDistant.Core.Config;
using SociallyDistant.Core.Config.SystemConfigCategories;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Config;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS;
using SociallyDistant.Core.OS.Network.MessageTransport;
using SociallyDistant.Core.Scripting;
using SociallyDistant.Core.Serialization.Binary;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.InfoPanel;
using SociallyDistant.Core.Social;
using SociallyDistant.Core.UI;
using SociallyDistant.DevTools;
using SociallyDistant.GamePlatform;
using SociallyDistant.GamePlatform.ContentManagement;
using SociallyDistant.GameplaySystems.Networld;
using SociallyDistant.GameplaySystems.Social;
using SociallyDistant.Modding;
using SociallyDistant.Player;
using SociallyDistant.UI;
using SociallyDistant.UI.Backdrop;

namespace SociallyDistant;

internal sealed class SociallyDistantGame :
	Game,
	IGameContext
{
	private static readonly string gameDataPath =
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "acidic light",
			"Socially Distant");

	private readonly        GuiSynchronizationContext    synchronizationContext = new();
	private readonly        GuiService                   gui;
	private static readonly WorkQueue                    globalSchedule = new();
	private static          SociallyDistantGame          instance       = null!;
	private readonly        DevToolsManager              devTools;
	private readonly        Subject<PlayerInfo>          playerInfoSubject = new();
	private readonly        IObservable<PlayerInfo>      playerInfoObservable;
	private readonly        TabbedToolCollection         tabbedTools;
	private readonly        GraphicsDeviceManager        graphicsManager;
	private readonly        TimeData                     timeData;
	private readonly        IObservable<GameMode>        gameModeObservable;
	private readonly        Subject<GameMode>            gameModeSubject = new();
	private readonly        ModuleManager                moduleManager;
	private readonly        WorldManager                 worldManager;
	private readonly        SocialService                socialService;
	private readonly        UriManager                   uriManager;
	private readonly        PlayerManager                playerManager;
	private readonly        NetworkController            network;
	private readonly        ContentManager               contentManager;
	private readonly        SettingsManager              settingsManager;
	private readonly        DeviceCoordinator            deviceCoordinator;
	private readonly        ScriptSystem                 scriptSystem;
	private readonly        VertexPositionColorTexture[] virtualScreenVertices = new VertexPositionColorTexture[4];
	private readonly        int[]                        virtualScreenIndices  = new[] { 0, 1, 2, 2, 1, 3 };
	private readonly        BackdropController           backdrop;
	private readonly        BackdropUpdater              backdropUpdater;
	private readonly        GuiController                guiController;

	private bool            areModulesLoaded;
	private Task            initializeTask;
	private PlayerInfo      playerInfo = new();
	private bool            initialized;
	private RenderTarget2D? virtualScreen;
	private SpriteEffect?   virtualScreenShader;
	private IGameData?      currentGameData;
	private PlayerInfo      loadedPlayerInfo;

	public bool IsGameActive => CurrentGameMode == GameMode.OnDesktop;

	/// <inheritdoc />
	public IModuleManager ModuleManager => moduleManager;

	/// <inheritdoc />
	public TabbedToolCollection AvailableTools => tabbedTools;
	
	/// <inheritdoc />
	public GameMode CurrentGameMode { get; private set; } = GameMode.Booting;

	/// <inheritdoc />
	public ISocialService SocialService => socialService;

	/// <inheritdoc />
	public string? CurrentSaveDataDirectory { get; private set; }

	/// <inheritdoc />
	public IUriManager UriManager => uriManager;

	/// <inheritdoc />
	public IKernel Kernel { get; }

	/// <inheritdoc />
	public IShellContext Shell => guiController;

	/// <inheritdoc />
	public Game GameInstance => this;

	/// <inheritdoc />
	public IContentManager ContentManager => contentManager;
	
	/// <inheritdoc />
	public IWorldManager WorldManager => worldManager;

	/// <inheritdoc />
	public ISettingsManager SettingsManager => settingsManager;

	/// <inheritdoc />
	public IScriptSystem ScriptSystem => scriptSystem;

	public IObservable<GameMode> GameModeObservable => gameModeObservable;

	public IObservable<PlayerInfo> PlayerInfoObservable => playerInfoObservable;

	internal SociallyDistantGame()
	{
		instance = this;

		timeData = Time.Initialize();
		graphicsManager = new GraphicsDeviceManager(this);
		graphicsManager.GraphicsProfile = GraphicsProfile.HiDef;

		tabbedTools = new TabbedToolCollection(this);

		gameModeObservable = Observable.Create<GameMode>((observer) =>
		{
			observer.OnNext(CurrentGameMode);
			return gameModeSubject.Subscribe(observer);
		});

		playerInfoObservable = Observable.Create<PlayerInfo>((observer) =>
		{
			observer.OnNext(playerInfo);
			return playerInfoSubject.Subscribe(observer);
		});

		var contentPipeline = new ContentPipeline(this.Services);

		this.backdrop = new BackdropController(this);
		this.backdropUpdater = new BackdropUpdater(this);
		this.devTools = new DevToolsManager(this);
		this.settingsManager = new SettingsManager();
		this.contentManager = new ContentManager(this, contentPipeline);
		this.moduleManager = new ModuleManager(this);
		this.worldManager = new WorldManager(this);
		this.network = new NetworkController(this);
		this.uriManager = new UriManager(this);
		this.scriptSystem = new ScriptSystem(this);
		this.socialService = new SocialService();
		this.gui = new GuiService(this);
		this.deviceCoordinator = new DeviceCoordinator(this);

		Components.Add(deviceCoordinator);
		Components.Add(network);
		Components.Add(backdrop);
		Components.Add(backdropUpdater);
		Components.Add(gui);

		var playerLan = network.Simulation.CreateLocalAreaNetwork();
		this.playerManager = new PlayerManager(this, deviceCoordinator, playerLan);

		this.guiController = new GuiController(this, playerManager);
		Components.Add(guiController);
		Components.Add(devTools);



		IsMouseVisible = true;

		graphicsManager.HardwareModeSwitch = false;
		graphicsManager.PreparingDeviceSettings += OnGraphicsDeviceCreation;

		Content = contentPipeline;

		contentPipeline.AddDirectoryContentSource("/Core", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content"));
	}

	private void OnGraphicsDeviceCreation(object? sender, PreparingDeviceSettingsEventArgs e)
	{
		settingsManager.Load();

		var graphicsSettings = new GraphicsSettings(settingsManager);

		var presentationParameters = e.GraphicsDeviceInformation.PresentationParameters;

		ApplyGraphicsSettingsInternal(graphicsSettings, presentationParameters, false);
	}

	protected override void Initialize()
	{
		base.Initialize();

		var graphicsSettings = new GraphicsSettings(settingsManager);
		graphicsManager.IsFullScreen = graphicsSettings.Fullscreen;
		graphicsManager.ApplyChanges();

		ApplyVirtualDisplayMode(graphicsSettings);

		initializeTask = InitializeAsync();
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		SetGameMode(GameMode.Loading);
        
		settingsManager.Save();
		base.OnExiting(sender, args);
	}

	private async Task InitializeAsync()
	{
		scriptSystem.RegisterHookListener(CommonScriptHooks.AfterContentReload,
			new UpdateAvailableToolsHook(contentManager, this.AvailableTools));

		await WaitForModulesToLoad(true);
		await contentManager.RefreshContentDatabaseAsync();

		settingsManager.ObserveChanges(OnGameSettingsChanged);

		await DoUserInitialization();
	}

	public async Task WaitForModulesToLoad(bool doReload)
	{
		if (areModulesLoaded && !doReload)
			return;

		areModulesLoaded = false;

		await moduleManager.LocateAllGameModules();

		areModulesLoaded = true;
	}

	private async Task DoUserInitialization()
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

		if (flow == InitializationFlow.MostRecentSave)
			await StartCharacterCreator();
		else 
			await GoToLoginScreen();
	}

	public InitializationFlow GetInitializationFlow()
	{
		var modSettings = new ModdingSettings(settingsManager);
		var uiSettings = new UiSettings(settingsManager);

		if (modSettings.ModDebugMode)
			return InitializationFlow.DebugWorld;

		return uiSettings.LoadMostRecentSave ? InitializationFlow.MostRecentSave : InitializationFlow.LoginScreen;
	}

	/// <inheritdoc />
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

			while (globalSchedule.Count > 0)
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

			//await gameInitializationScript.ExecuteAsync(unityConsole);
		}
		catch (Exception ex)
		{
			this.guiController.ShowExceptionMessage(ex);

			await EndCurrentGame(false);
			await GoToLoginScreen();
			return;
		}

		playerInfoSubject.OnNext(this.loadedPlayerInfo);
		SetGameMode(GameMode.OnDesktop);
	}

	/// <inheritdoc />
	public async Task StartCharacterCreator()
	{
		await EndCurrentGame(true);

		SetGameMode(GameMode.CharacterCreator);
	}

	/// <inheritdoc />
	public async Task GoToLoginScreen()
	{
		await EndCurrentGame(true);

		SetGameMode(GameMode.AtLoginScreen);
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

		//await gameInitializationScript.ExecuteAsync();
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

		worldManager.WipeWorld();
	}

	/// <inheritdoc />
	public bool IsDebugWorld => currentGameData is DebugGameData;

	private void SetGameMode(GameMode newGameMode)
	{
		CurrentGameMode = newGameMode;
		this.gameModeSubject.OnNext(newGameMode);
	}

	/// <inheritdoc />
	public async void SetPlayerHostname(string hostname)
	{
		this.loadedPlayerInfo.HostName = hostname;
		this.playerInfoSubject.OnNext(this.loadedPlayerInfo);

		await this.SaveCurrentGame(true);
	}

	/// <inheritdoc />
	protected override void Update(GameTime gameTime)
	{
		// Run any scheduled actions
		globalSchedule.RunPendingWork();

		// Update the synchronization context
		synchronizationContext.Update();
		
		// Report new timing data to the rest of the game so it can be accessed statically
		timeData.Update(gameTime);

		if (!initialized && initializeTask.IsCompleted)
		{
			if (!initializeTask.IsCompletedSuccessfully)
			{
				Log.Error("Game initialization failed. We're about to exit.");
				if (initializeTask.Exception != null)
					Log.Error(initializeTask.Exception.ToString());

				Exit();
			}

			initialized = true;
		}

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.Black);
		base.Draw(gameTime);
	}

	public static SociallyDistantGame Instance => instance;
	public static string GameDataPath => gameDataPath;

	public static void Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.CreateLogger();

		AppDomain.CurrentDomain.UnhandledException += Fuck;

		void Fuck(object sender, UnhandledExceptionEventArgs e)
		{
			Log.Error("Fuck. Game just crashed.");
			Log.Fatal(e.ExceptionObject.ToString() ?? "Unknown exception details.");
		}

		try
		{
			using var game = new GameApplication();
			game.Run();
		}
		finally
		{
			Log.CloseAndFlush();
		}
	}

	public static void ScheduleAction(Action action)
	{
		globalSchedule.Enqueue(action);
	}

	private void ApplyVirtualDisplayMode(GraphicsSettings settings)
	{
		if (!TryParseResolution(settings.DisplayResolution, out DisplayMode mode))
		{
			Log.Warning("Resolution stored in settings is missing or unsupported, so using default.");
			settings.DisplayResolution = $"{mode.Width}x{mode.Height}";
		}

		gui.SetVirtualScreenSize(mode.Width, mode.Height);
	}

	private bool TryParseResolution(string? resolution, out DisplayMode displayMode)
	{
		displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

		if (string.IsNullOrWhiteSpace(resolution))
			return false;

		string[] parts = resolution.Split('x');

		if (parts.Length != 2)
			return false;

		if (!int.TryParse(parts[0], out int width))
			return false;

		if (!int.TryParse(parts[1], out int height))
			return false;

		var supportedModes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes
			.Where(x => x.Width == width && x.Height == height).ToArray();

		if (supportedModes.Length == 0)
			return false;

		displayMode = supportedModes.First();
		return true;
	}

	private void ApplyGraphicsSettingsInternal(GraphicsSettings settings, PresentationParameters parameters,
		bool explicitApply)
	{
		var defaultScreenSize = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

		// When going into fullscreen mode, we render to a virtual display and blit it.
		if (settings.Fullscreen)
		{
			parameters.BackBufferWidth = defaultScreenSize.Width;
			parameters.BackBufferHeight = defaultScreenSize.Height;
		}
		else
		{
			if (!TryParseResolution(settings.DisplayResolution, out DisplayMode mode))
			{
				Log.Warning("Resolution stored in settings is missing or unsupported, so using default.");
				settings.DisplayResolution = $"{mode.Width}x{mode.Height}";
			}

			parameters.BackBufferWidth = mode.Width;
			parameters.BackBufferHeight = mode.Height;
		}

		parameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

		if (explicitApply)
		{
			graphicsManager.ApplyChanges();
			ApplyVirtualDisplayMode(settings);
		}
	}

	private void OnGameSettingsChanged(ISettingsManager settings)
	{
		// Not yet.
		if (GraphicsDevice == null)
			return;

		var graphicsSettings = new GraphicsSettings(settings);
		var parameters = GraphicsDevice.PresentationParameters;

		ApplyGraphicsSettingsInternal(graphicsSettings, parameters, true);
	}

	private sealed class UpdateAvailableToolsHook : IHookListener
	{
		private readonly IContentManager contentManager;
		private readonly TabbedToolCollection collection;
		private readonly MainToolGroup? terminal;

		public UpdateAvailableToolsHook(IContentManager contentManager, TabbedToolCollection collection)
		{
			this.contentManager = contentManager;
			this.collection = collection;
		}

		/// <inheritdoc />
		public async Task ReceiveHookAsync(IGameContext game)
		{
			collection.Clear();

			if (terminal != null)
				collection.Add(terminal);

			foreach (ITabbedToolDefinition group in contentManager.GetContentOfType<ITabbedToolDefinition>())
			{
				collection.Add(group);
			}

			await game.ScriptSystem.RunHookAsync(CommonScriptHooks.BeforeUpdateShellTools);
		}
	}
}