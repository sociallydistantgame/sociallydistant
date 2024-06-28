using SociallyDistant.Core.Core.Config;

namespace SociallyDistant.Core.Config.SystemConfigCategories;

[SettingsCategory("ui", "Interface Preferences", CommonSettingsCategorySections.Interface)]
public class UiSettings : SettingsCategory
{
    public UiSettings(ISettingsManager settingsManager) : base(settingsManager)
    {
    }
}