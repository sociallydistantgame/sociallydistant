#nullable enable
using Core.Config;

namespace UI.SystemSettings
{
	public class SettingsCategoryModel
	{
		public string Title { get; set; } = string.Empty;
		public string? MetaTitle { get; set; }
		public bool ShowTitleArea { get; set; }
		public SettingsCategory? Category { get; set; }
	}
}