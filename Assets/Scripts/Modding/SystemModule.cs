#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Config;
using Core.Config.SystemConfigCategories;
using GamePlatform.ContentManagement;
using GameplaySystems.WebPages;
using Modules;
using Shell;
using UI.PlayerUI;
using UnityEngine;
using Utility;

namespace Modding
{
	/// <summary>
	///		The system module. This is Socially Distant itself.
	/// </summary>
	[IgnoreModdingLegalWaiver]
	public class SystemModule : GameModule
	{
		private readonly WebSiteContentManager websites = new();
		private GraphicsSettings? graphicsSettings;
		private AccessibilitySettings? a11ySettings;
		private UiSettings uiSettings;
		private IDisposable? settingsObservable;
		private UserThemeSource userThemeSource = new UserThemeSource();
		
		/// <inheritdoc />
		public override bool IsCoreModule => true;

		/// <inheritdoc />
		public override string ModuleId => "com.sociallydistant.system";

		/// <inheritdoc />
		protected override async Task OnInitialize()
		{
			// System settings modules
			graphicsSettings = Context.SettingsManager.RegisterSettingsCategory<GraphicsSettings>();
			a11ySettings = Context.SettingsManager.RegisterSettingsCategory<AccessibilitySettings>();
			uiSettings = Context.SettingsManager.RegisterSettingsCategory<UiSettings>();
			
			// Watch system settings
			settingsObservable = Context.SettingsManager.ObserveChanges(OnSettingsUpdated);
		
			// User-created themes
			Context.ContentManager.AddContentSource(userThemeSource);
			
			// Websites
			Context.ContentManager.AddContentSource(websites);

			// Find Restitched save data. If we do, the player gets a little gift from the lead programmer of the game - who.......created this game as well? <3
			// FindRestitchedDataAndRegisterRestitchedContent();
		}

		/// <inheritdoc />
		protected override async Task OnShutdown()
		{
			Context.ContentManager.RemoveContentSource(websites);
			
			if (userThemeSource != null)
				Context.ContentManager.RemoveContentSource(userThemeSource);
			
			if (graphicsSettings != null)
				Context.SettingsManager.UnregisterSettingsCategory(graphicsSettings);

			if (a11ySettings != null)
				Context.SettingsManager.UnregisterSettingsCategory(a11ySettings);

			userThemeSource = null;
			settingsObservable?.Dispose();
			settingsObservable = null;
		}

		private void OnSettingsUpdated(ISettingsManager settings)
		{
			
		}
		
		private void FindRestitchedDataAndRegisterRestitchedContent()
		{
			return;
			
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
				Context.InfoPanelService.CreateCloseableInfoWidget(MaterialIcons.Brush, "New content unlocked!", "The Hypervisor has detected saved game data on your host computer from Restitched. Some Restitched-themed content has been added to your OS. Go to System Settings -> Customization to see what's new.");
			}

            // Register Restitched content with ContentManager.
            Context.ContentManager.AddContentSource<RestitchedContentSource>();
		}
	}
}