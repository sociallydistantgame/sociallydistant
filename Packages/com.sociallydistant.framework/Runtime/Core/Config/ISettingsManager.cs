#nullable enable

namespace Core.Config
{
	public interface ISettingsManager
	{
		string GameDataPath { get; }
		bool IsInitialized { get; }

		SettingsCategory? FindSettingsByKey(string key);
		T? FindSettings<T>() where T : SettingsCategory;

		void Save();
		void Load();
		void ResetKey(string key);
		bool HasKey(string key);
		bool HasValue<T>(string key);
		void SetValue<T>(string key, T value);
		bool TryGetValue<T>(string key, out T? value);
		void GetValueOrDefault<T>(string key, T defaultValue, out T? value);
	}
}