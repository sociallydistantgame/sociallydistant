using System;
using System.Linq;

namespace Core.Config
{
	public abstract class SettingsCategory
	{
		private readonly ISettingsManager settingsManager;
		private SettingsCategoryAttribute reflectionAttribute;

		public string CategoryKey => reflectionAttribute.Key;
		public string Name => reflectionAttribute.DisplayName;
		public bool Hidden => reflectionAttribute.Hidden;
		
		protected SettingsCategory(ISettingsManager settingsManager)
		{
			this.settingsManager = settingsManager;

			this.LocateCategoryAttribute();
		}

		protected void SetValue(string name, bool value)
		{
			settingsManager.SetBool(GetFullyQualifiedSettingsKey(name), value);
		}
		
		protected void SetValue(string name, string value)
		{
			settingsManager.SetString(GetFullyQualifiedSettingsKey(name), value);
		}
		
		protected void SetValue(string name, int value)
		{
			settingsManager.SetInt(GetFullyQualifiedSettingsKey(name), value);
		}
		
		protected void SetValue(string name, float value)
		{
			settingsManager.SetFloat(GetFullyQualifiedSettingsKey(name), value);
		}

		protected bool GetValue(string key, bool defaultValue)
		{
			return settingsManager.GetBool(GetFullyQualifiedSettingsKey(key), defaultValue);
		}
		
		protected string GetValue(string key, string defaultValue)
		{
			return settingsManager.GetString(GetFullyQualifiedSettingsKey(key), defaultValue);
		}
		
		protected float GetValue(string key, float defaultValue)
		{
			return settingsManager.GetFloat(GetFullyQualifiedSettingsKey(key), defaultValue);
		}
		
		protected int GetValue(string key, int defaultValue)
		{
			return settingsManager.GetInt(GetFullyQualifiedSettingsKey(key), defaultValue);
		}

		private void LocateCategoryAttribute()
		{
			Type type = this.GetType();

			SettingsCategoryAttribute? attribute = type.GetCustomAttributes(false)
				.OfType<SettingsCategoryAttribute>()
				.FirstOrDefault();

			if (attribute == null)
				attribute = new SettingsCategoryAttribute($"com.sociallydistant.{type.Name}", type.Name, hidden: true);

			this.reflectionAttribute = attribute;
		}

		private string GetFullyQualifiedSettingsKey(string valueName)
		{
			return $"{CategoryKey}.{valueName}";
		}
	}
}