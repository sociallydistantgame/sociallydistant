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

		protected void SetValue<T>(string name, T value)
		{
			settingsManager.SetValue(GetFullyQualifiedSettingsKey(name), value);
		}

		protected T GetValue<T>(string key, T defaultValue)
		{
			settingsManager.GetValueOrDefault(GetFullyQualifiedSettingsKey(key), defaultValue, out T value);
			return value;
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