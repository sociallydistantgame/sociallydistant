#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Core;
using Core.Config;
using GamePlatform;
using Modules;
using UnityEngine;
using UniRx;

namespace Modding
{
	public class ModuleManager
	{
		private readonly string modsDirectory = Path.Combine(Application.persistentDataPath, "mods");
		private readonly List<GameModule> allModules = new List<GameModule>();
		private readonly Dictionary<string, GameModule> moduleIds = new Dictionary<string, GameModule>();
		private readonly IGameContext gameContext;
		private readonly ModdingSettings moddingSettings;
		private readonly IDisposable gameModeObserver;

		private GameMode gameMode;
		
		public ModuleManager(GameManager context)
		{
			this.gameContext = context;
			moddingSettings = this.gameContext.SettingsManager.RegisterSettingsCategory<ModdingSettings>();
			this.gameModeObserver = context.GameModeObservable.Subscribe(OnGameModeChanged);
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
					Debug.LogException(ex);
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
				Debug.LogException(ex);
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
					Debug.LogError($"Module {module.ModuleId} depends on a missing module: {dependencyId}");
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
						
						Debug.Log("Found external game module: " + gameModule.ModuleId);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Cannot load mod at path " + file);
					Debug.LogException(ex);
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
	}
}