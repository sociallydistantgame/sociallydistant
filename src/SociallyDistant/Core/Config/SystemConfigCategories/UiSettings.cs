using SociallyDistant.Core.Core.Config;

namespace SociallyDistant.Core.Config.SystemConfigCategories;

[SettingsCategory("ui", "Interface Preferences", CommonSettingsCategorySections.Interface)]
public class UiSettings : SettingsCategory
{
    public bool LoadMostRecentSave
    {
        get => GetValue(nameof(LoadMostRecentSave), true);
        set => SetValue(nameof(LoadMostRecentSave), value);
    }
    
    public UiSettings(ISettingsManager settingsManager) : base(settingsManager)
    {
    }
}