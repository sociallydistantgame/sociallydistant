#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Config
{
	public interface ISettingsManager
	{
		string GameDataPath { get; }
		bool IsInitialized { get; }

		SettingsCategory? FindSettingsByKey(string key);
		T? FindSettings<T>() where T : SettingsCategory;
        
		IEnumerable<string> SectionTitles { get; }
		IEnumerable<string> Keys { get; }

		IEnumerable<SettingsCategory> GetCategoriesInSection(string sectionTitle);
		
		bool GetBool(string key, bool defaultValue = false);
		string? GetString(string key, string? defaultValue = null);
		float GetFloat(string key, float defaultValue = 0);
		int GetInt(string key, int defaultValue = 0);

		bool IsBool(string key);
		bool IsString(string key);
		bool IsFloat(string key);
		bool IsInt(string key);

		void SetFloat(string key, float value);
		void SetString(string key, string value);
		void SetInt(string key, int value);
		void SetBool(string key, bool value);
		
		void Save();
		void Load();
		void ResetKey(string key);
		bool HasKey(string key);

		IDisposable ObserveChanges(Action<ISettingsManager> onUpdate);

		T RegisterSettingsCategory<T>() where T : SettingsCategory;
		void UnregisterSettingsCategory(SettingsCategory category);
	}
}