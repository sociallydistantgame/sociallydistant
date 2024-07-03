using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using SociallyDistant.Core;
using SociallyDistant.Core.Config;
using SociallyDistant.Core.Config.SystemConfigCategories;
using SociallyDistant.Core.ContentManagement;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Config;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS;
using SociallyDistant.Core.OS.Network.MessageTransport;
using SociallyDistant.Core.Scripting;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.InfoPanel;
using SociallyDistant.Core.Social;
using SociallyDistant.Core.UI;
using SociallyDistant.DevTools;
using SociallyDistant.GamePlatform;
using SociallyDistant.GamePlatform.ContentManagement;
using SociallyDistant.GameplaySystems.Social;
using SociallyDistant.Modding;
using SociallyDistant.UI.Backdrop;

namespace SociallyDistant;

internal sealed class SociallyDistantGame :
	Game,
	IGameContext
{
	private static readonly string gameDataPath =
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "acidic light",
			"Socially Distant");

	private readonly GuiService gui;
	private static readonly WorkQueue globalSchedule = new();
	private static SociallyDistantGame instance = null!;
	private readonly DevToolsManager devTools;
	private readonly Subject<PlayerInfo> playerInfoSubject = new();
	private readonly IObservable<PlayerInfo> playerInfoObservable;
	private readonly TabbedToolCollection tabbedTools;
	private readonly GraphicsDeviceManager graphicsManager;
	private readonly TimeData timeData;
	private readonly IObservable<GameMode> gameModeObservable;
	private readonly Subject<GameMode> gameModeSubject = new();
	private readonly ModuleManager moduleManager;
	private readonly WorldManager worldManager;
	private readonly SocialService socialService;
	private readonly UriManager uriManager;
	private readonly ContentManager contentManager;
	private readonly SettingsManager settingsManager;
	private readonly ScriptSystem scriptSystem;
    private readonly VertexPositionColorTexture[] virtualScreenVertices = new VertexPositionColorTexture[4];
    private readonly int[] virtualScreenIndices = new[] { 0, 1, 2, 2, 1, 3 };
    private readonly BackdropController backdrop;
    private readonly BackdropUpdater backdropUpdater;

	private Task initializeTask;
	private PlayerInfo playerInfo = new();
	private bool initialized;
	private RenderTarget2D? virtualScreen;
	private SpriteEffect? virtualScreenShader;

	public bool IsGameActive => CurrentGameMode == GameMode.OnDesktop;
	
	/// <inheritdoc />
	public IModuleManager ModuleManager => moduleManager;

	/// <inheritdoc />
	public TabbedToolCollection AvailableTools => tabbedTools;

	/// <inheritdoc />
	public INotificationManager NotificationManager { get; }

	/// <inheritdoc />
	public GameMode CurrentGameMode { get; private set; } = GameMode.Loading;

	/// <inheritdoc />
	public ISocialService SocialService => socialService;

	/// <inheritdoc />
	public string? CurrentSaveDataDirectory { get; private set; }

	/// <inheritdoc />
	public IUriManager UriManager => uriManager;

	/// <inheritdoc />
	public IKernel Kernel { get; }

	/// <inheritdoc />
	public IShellContext Shell { get; }

	/// <inheritdoc />
	public Game GameInstance => this;
	
	/// <inheritdoc />
	public IContentManager ContentManager => contentManager;

	/// <inheritdoc />
	public IInfoPanelService InfoPanelService { get; }

	/// <inheritdoc />
	public IWorldManager WorldManager => worldManager;

	/// <inheritdoc />
	public ISettingsManager SettingsManager => settingsManager;

	/// <inheritdoc />
	public IScriptSystem ScriptSystem => scriptSystem;

	public IObservable<GameMode> GameModeObservable => gameModeObservable;

	public IObservable<PlayerInfo> PlayerInfoObservable => playerInfoObservable;
	
	private SociallyDistantGame()
	{
		instance = this;
		
		timeData = Time.Initialize();
		graphicsManager = new GraphicsDeviceManager(this);

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
		this.uriManager = new UriManager(this);
		this.scriptSystem = new ScriptSystem(this);
		this.socialService = new SocialService();
		this.gui = new GuiService(this);

		Components.Add(backdrop);
		Components.Add(backdropUpdater);
		//Components.Add(gui);
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
		settingsManager.Save();
		base.OnExiting(sender, args);
	}

	private async Task InitializeAsync()
	{
		await moduleManager.LocateAllGameModules();
		await contentManager.RefreshContentDatabaseAsync();

		settingsManager.ObserveChanges(OnGameSettingsChanged);
	}

	/// <inheritdoc />
	public async Task SaveCurrentGame(bool silent)
	{
		
	}

	/// <inheritdoc />
	public async Task EndCurrentGame(bool save)
	{
	}

	/// <inheritdoc />
	public bool IsDebugWorld { get; }

	/// <inheritdoc />
	public void SetPlayerHostname(string hostname)
	{
	}

	/// <inheritdoc />
	protected override void Update(GameTime gameTime)
	{
		// Run any scheduled actions
		globalSchedule.RunPendingWork();
		
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
		
		using var game = new SociallyDistantGame();

		try
		{
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

		if (explicitApply)
		{
			graphicsManager.ApplyChanges();
			ApplyVirtualDisplayMode(settings);
		}
	}

	private void OnGameSettingsChanged(ISettingsManager settings)
	{
		var graphicsSettings = new GraphicsSettings(settings);
		var parameters = GraphicsDevice.PresentationParameters;
		
		ApplyGraphicsSettingsInternal(graphicsSettings, parameters, true);
	}
}