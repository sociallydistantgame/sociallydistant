using System;

namespace Core.Config
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SettingsCategoryAttribute : Attribute
	{
		public SettingsCategoryAttribute(string key, string displayName, bool hidden = false)
		{
			this.Key = key;
			DisplayName = displayName;
			Hidden = hidden;
		}
		
		public string Key { get; }
		public string DisplayName { get; }
		public bool Hidden { get; }
	}
}