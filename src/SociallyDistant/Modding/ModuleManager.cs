#nullable enable

using System.Diagnostics;
using System.Net.Mime;
using System.Reflection;
using Serilog;
using SociallyDistant.Core.Config;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Core.Config;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell.Commands;

namespace SociallyDistant.Modding
{
	public class ModuleManager : IModuleManager
	{
		private readonly string modsDirectory = Path.Combine(SociallyDistantGame.GameDataPath, "mods");
		private readonly List<GameModule> allModules = new List<GameModule>();
		private readonly Dictionary<string, GameModule> moduleIds = new Dictionary<string, GameModule>();
		private readonly IGameContext gameContext;
		private readonly ModdingSettings moddingSettings;
		private readonly IDisposable gameModeObserver;
		private readonly CustomCommandManager customCommandManager;

		private IDisposable? settingsObserver;
		private bool isInDebugMode;
		
		private GameMode gameMode;
		
		internal ModuleManager(SociallyDistantGame context)
		{
			this.gameContext = context;
			moddingSettings = this.gameContext.SettingsManager.RegisterSettingsCategory<ModdingSettings>();
			this.gameModeObserver = context.GameModeObservable.Subscribe(OnGameModeChanged);
			this.customCommandManager = new CustomCommandManager(this);

			this.gameContext.ContentManager.AddContentGenerator(customCommandManager);
		}

		private async void OnGameSettingsChanged(ISettingsManager settingsManager)
		{
			var modSettings = new ModdingSettings(settingsManager);

			if (isInDebugMode == modSettings.ModDebugMode)
				return;

			await gameContext.Shell.ShowInfoDialog(
				"System message",
				modSettings.ModDebugMode
					? "You have enabled Mod Debug Mode. The game will now exit."
					: "You have disabled Mod Debug Mode. The game will now exit."
			);

			await gameContext.SaveCurrentGame(true);
			
			// PlatformHelper.QuitToDesktop();
		}

		internal async Task LocateAllGameModules()
		{
			this.allModules.Clear();
			
			// Us
			this.allModules.Add(new SystemModule());

			// We only locate mod DLLs if the player has accepted the legal waiver that lets us potentially load malware into RAM.
			if (moddingSettings.AcceptLegalWaiver)
				await Task.Run(LocateMods);

			await InitializeModules();

			isInDebugMode = new ModdingSettings(gameContext.SettingsManager).ModDebugMode;

			if (settingsObserver == null)
				settingsObserver = this.gameContext.SettingsManager.ObserveChanges(OnGameSettingsChanged);
		}

		public async Task Shutdown()
		{
			foreach (GameModule module in allModules)
			{
				try
				{
					await module.Shutdown();
				}
				catch (Exception ex)
				{
					Log.Error(ex.Message);
				}
			}

			allModules.Clear();
			moduleIds.Clear();

			gameModeObserver.Dispose();
		}
		
		private async Task InitializeModules()
		{
			foreach (GameModule module in allModules)
			{
				await LoadModule(module);
			}
		}

		private async Task<bool> LoadModule(GameModule module)
		{
			bool couldLoadDependencies = await LoadDependencies(module);
			if (!couldLoadDependencies)
				return false;

			// If the player hasn't accepted the legal agreement for script mods, make sure we only load modules that have the [IgnoreModdingLegalWaiver] attribute.
			// This effectively only allows the game to load modules that I wrote.
			if (!moddingSettings.AcceptLegalWaiver)
			{
				Type moduleType = module.GetType();

				IgnoreModdingLegalWaiverAttribute? attribute = moduleType.GetCustomAttributes(false)
					.OfType<IgnoreModdingLegalWaiverAttribute>()
					.FirstOrDefault();

				if (attribute == null)
					return false;
			}
			
			try
			{
				await module.Initialize(this.gameContext);
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
				return false;
			}

			return true;
		}
		
		private async Task<bool> LoadDependencies(GameModule module)
		{
			foreach (string dependencyId in module.RequiredModules)
			{
				if (!moduleIds.ContainsKey(dependencyId))
				{
					Log.Error($"Module {module.ModuleId} depends on a missing module: {dependencyId}");
					return false;
				}

				bool couldLoadDependency = await LoadModule(moduleIds[dependencyId]);

				if (!couldLoadDependency)
					return false;
			}

			return true;
		}
		

		private void LocateMods()
		{
			if (!Directory.Exists(modsDirectory))
			{
				// Mods directory doesn't exist yet, there are no mods.
				Directory.CreateDirectory(modsDirectory);
				return;
			}

			foreach (string? file in Directory.EnumerateFiles(modsDirectory, "*.dll", SearchOption.AllDirectories))
			{
				if (string.IsNullOrWhiteSpace(file))
					continue;

				try
				{
					Assembly modAssembly = Assembly.LoadFrom(file);
					
					// Find all non-abstract GameModule types
					IEnumerable<Type> types = modAssembly.GetTypes()
						.Where(x => typeof(GameModule).IsAssignableFrom(x))
						.Where(x => x.GetConstructor(Type.EmptyTypes) != null);

					foreach (Type type in types)
					{
						var gameModule = Activator.CreateInstance(type, null) as GameModule;
						if (gameModule == null)
							continue;

						if (this.moduleIds.ContainsKey(gameModule.ModuleId))
							throw new InvalidOperationException("Two modules with the same module ID were detected. The module loader will not load the second one. The game might break.");

						this.moduleIds.Add(gameModule.ModuleId, gameModule);
						allModules.Add(gameModule);
						
						Log.Information("Found external game module: " + gameModule.ModuleId);
					}
				}
				catch (Exception ex)
				{
					Log.Error("Cannot load mod at path " + file);
					Log.Error(ex.ToString());
				}
			}
		}

		private void OnGameModeChanged(GameMode gameMode)
		{
			this.gameMode = gameMode;

			foreach (GameModule module in allModules)
			{
				if (!module.IsInitialized)
					continue;
				
				module.OnGameModeChanged(gameMode);
			}
		}

		/// <inheritdoc />
		public IEnumerable<GameModule> Modules => this.allModules;
	}
}