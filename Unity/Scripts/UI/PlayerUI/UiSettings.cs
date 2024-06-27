#nullable enable
using Core.Config;
using GamePlatform;
using UnityEditor;
using UnityEngine.InputSystem.Layouts;

namespace UI.PlayerUI
{
	[SettingsCategory("ui", "Interface Preferences", CommonSettingsCategorySections.Interface)]
	public class UiSettings : SettingsCategory
	{
		public DayNightMode TerminalColorMode
		{
			get => (DayNightMode) GetValue(nameof(TerminalColorMode), (int) DayNightMode.Adaptive);
			set => SetValue(nameof(TerminalColorMode), (int) value);
		}
		
		public DayNightMode WallpaperMode
		{
			get => (DayNightMode) GetValue(nameof(WallpaperMode), (int) DayNightMode.Adaptive);
			set => SetValue(nameof(WallpaperMode), (int) value);
		}

		public bool ShowTerminalOnLogin
		{
			get => GetValue(nameof(ShowTerminalOnLogin), true);
			set => SetValue(nameof(ShowTerminalOnLogin), value);
		}
		
		public bool ShowNetOnLogin
		{
			get => GetValue(nameof(ShowNetOnLogin), true);
			set => SetValue(nameof(ShowNetOnLogin), value);
		}
		
		public bool VimMode
		{
			get => GetValue(nameof(VimMode), false);
			set => SetValue(nameof(VimMode), value);
		}

		public bool DisableGuiScaling
		{
			get => GetValue(nameof(DisableGuiScaling), false);
			set => SetValue(nameof(DisableGuiScaling), value);
		}

		public bool GnomeMode
		{
			get => GetValue(nameof(GnomeMode), false);
			set => SetValue(nameof(GnomeMode), value);
		}

		public GameManager.InitializationFlow PreferredInitializationFlow
		{
			get => (GameManager.InitializationFlow) GetValue(nameof(PreferredInitializationFlow), (int)GameManager.InitializationFlow.MostRecentSave);
			set => SetValue(nameof(PreferredInitializationFlow), (int) value);
		}

		public bool SkipAssCovering
		{
			get => GetValue(nameof(SkipAssCovering), false);
			set => SetValue(nameof(SkipAssCovering), value);
		}
		
		/// <inheritdoc />
		public UiSettings(ISettingsManager settingsManager) : base(settingsManager)
		{
		}

		/// <inheritdoc />
		public override void BuildSettingsUi(ISettingsUiBuilder uiBuilder)
		{
			var dayNightModeChoices = new string[]
			{
				"Adaptive",
				"Always day",
				"Always night"
			};
			
			uiBuilder.AddSection("Day/Night Cycle", out int dayNightCycle)
				.WithLabel("Change how certain interface elements react to the game's day/night cycle", dayNightCycle)
				.WithStringDropdown(
					"Terminal",
					"Determines what colors the Terminal uses",
					(int) TerminalColorMode,
					dayNightModeChoices,
					m => TerminalColorMode = (DayNightMode) m,
					dayNightCycle
				).WithStringDropdown(
					"Wallpaper",
					"Determines what wallpaper shows during day and night",
					(int) WallpaperMode,
					dayNightModeChoices,
					m => WallpaperMode = (DayNightMode) m,
					dayNightCycle
				);

			uiBuilder.AddSection("Navigation", out int navigation)
				.WithToggle(
					"Show Terminal on login",
					"When loading a new save game, immediately open the Terminal",
					ShowTerminalOnLogin,
					v => ShowTerminalOnLogin = v,
					navigation
				).WithToggle(
					"Show network view on login",
					"When loading a new save game, show the Network View tile",
					ShowNetOnLogin,
					v => ShowNetOnLogin = v,
					navigation
				).WithToggle(
					"VIM mode",
					"Use VIM-style editing in the Terminal (for advanced players only)",
					VimMode,
					v => VimMode = v,
					navigation
				);

			uiBuilder.AddSection("Compatibility", out int compatibility)
				.WithToggle(
					"Disable in-game scaling",
					"Some users may prefer to have the game's interface match the scale of their desktop. This setting turns off the game's scaler, rendering elements at 1:1 scale with your screen. <b>Warning:</b> This may reduce the legibility of the game's interface.",
					DisableGuiScaling,
					v => DisableGuiScaling = v,
					compatibility
				).WithToggle(
					"In-game window decorations",
					"Some Wayland compositors may not support the XDG decoration protocol, leading to the game window not having a title bar. Enabling this setting turns the game's Status Bar into a title bar for the game window.",
					GnomeMode,
					v => GnomeMode = v,
					compatibility
				);

			var flowChoices = new string[]
			{
				"Open the Welcome screen",
				"Load last-played game"
			};

			GameManager.InitializationFlow currentFlow = this.PreferredInitializationFlow;

			var modSettings = new ModdingSettings(this.Context);
			
			if (modSettings.ModDebugMode)
			{
				uiBuilder.AddSection("Startup", out int startup)
					.WithLabel("You are in Mod Debug mode. Startup settings cannot be changed.", startup);
			}
			else
			{
				uiBuilder.AddSection("Startup", out int startup)
					.WithStringDropdown(
						"Startup flow",
						"Choose how Socially Distant starts up by default.",
						(int) currentFlow,
						flowChoices,
						f => this.PreferredInitializationFlow = (GameManager.InitializationFlow) f,
						startup
					).WithToggle(
						"Skip legal disclaimers",
						"Skip the legal disclaimers and other text during startup.",
						SkipAssCovering,
						x => SkipAssCovering = x,
						startup
					);
			}
		}

		public enum DayNightMode : byte
		{
			Adaptive,
			Day,
			Night
		}
	}
}