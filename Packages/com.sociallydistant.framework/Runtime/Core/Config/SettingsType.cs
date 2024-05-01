namespace Core.Config
{
	public enum SettingsType
	{
		Boolean,
		String,
		Int,
		Float
	}

	public struct SettingsFieldUiDefinition
	{
		public string Key;
		public string DisplayName;
		public string Description;
		public string Section;
		public string Icon;
		public SettingsType Type;
	}
}