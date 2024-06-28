namespace SociallyDistant.Core.Core.Config
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SettingsCategoryAttribute : Attribute
	{
		public SettingsCategoryAttribute(string key, string displayName, string sectionName, bool hidden = false)
		{
			this.Key = key;
			DisplayName = displayName;
			Hidden = hidden;
			SectionName = sectionName;
		}
		
		public string SectionName { get; }
		public string Key { get; }
		public string DisplayName { get; }
		public bool Hidden { get; }
	}
}