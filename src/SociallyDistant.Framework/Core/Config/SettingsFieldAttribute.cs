using SociallyDistant.Core.Shell;

namespace SociallyDistant.Core.Core.Config
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SettingsFieldAttribute : Attribute
	{
		public SettingsFieldAttribute(string name, string? description = null, string? sectionTitle = null, string? icon = null)
		{
			Name = name;
			Description = description ?? string.Empty;
			SectionTitle = sectionTitle ?? CommonSettingsSections.General;
			Icon = icon ?? MaterialIcons.Settings;
		}
		
		public string Name { get; }
		public string Description { get; }
		public string SectionTitle { get; }
		public string Icon { get; }
	}
}