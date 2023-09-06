#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shell;
using UnityEngine;

namespace Core.Config
{
	public class SettingsManager : 
		ISettingsManager,
		IDisposable
	{
		private readonly Dictionary<string, SettingsCategory> categories = new Dictionary<string, SettingsCategory>();

		public SettingsManager()
		{
			Registry.Initialize();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			Registry.Shutdown();
		}
		
		/// <inheritdoc />
		public string GameDataPath => Registry.GameDataPath;

		/// <inheritdoc />
		public bool IsInitialized => Registry.IsInitialized;
		
		/// <inheritdoc />
		public SettingsCategory? FindSettingsByKey(string key)
		{
			return !categories.TryGetValue(key, out SettingsCategory? category) ? null : category;
		}

		/// <inheritdoc />
		public T? FindSettings<T>() where T : SettingsCategory
		{
			return categories.Values.OfType<T>().FirstOrDefault();
		}

		/// <inheritdoc />
		public void Save()
		{
			Registry.SaveRegistry();
		}

		/// <inheritdoc />
		public void Load()
		{
			Registry.Reload();
		}

		/// <inheritdoc />
		public void ResetKey(string key)
		{
			Registry.ResetKey(key);
		}

		/// <inheritdoc />
		public bool HasKey(string key)
		{
			return Registry.HasKey(key);
		}

		/// <inheritdoc />
		public bool HasValue<T>(string key)
		{
			return Registry.HasValue<T>(key);
		}

		/// <inheritdoc />
		public void SetValue<T>(string key, T value)
		{
			Registry.SetValue(key, value);
		}

		/// <inheritdoc />
		public bool TryGetValue<T>(string key, out T? value)
		{
			return Registry.TryGetValue(key, out value);
		}

		/// <inheritdoc />
		public void GetValueOrDefault<T>(string key, T defaultValue, out T? value)
		{
			Registry.GetValueOrSetDefault(key, defaultValue, out value);
		}
		
		
	}

	[SettingsCategory("com.sociallydistant.modding", "Mod settings")]
	[SystemSettings("Mods")]
	public class ModdingSettings : SettingsCategory
	{
		[SettingsField("Enable script mods", "Allow execution of script mods. By turning this on, you acknowledge that Socially Distant and its development team is not responsible for the content and behaviour of any installed mods, and that we accept no responsibility for any damage done to the game or your computer.", CommonSettingsSections.Legal)]
		public bool AcceptLegalWaiver
		{
			get => GetValue(nameof(AcceptLegalWaiver), false);
			set => SetValue(nameof(AcceptLegalWaiver), value);
		}

		/// <inheritdoc />
		public ModdingSettings(ISettingsManager settingsManager) : base(settingsManager)
		{ }
	}
}