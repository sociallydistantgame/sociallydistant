using System.Collections.Immutable;
using SociallyDistant.Core.Core.Config;

namespace SociallyDistant.Core.Config.SystemConfigCategories;

[SettingsCategory("ui", "Interface Preferences", CommonSettingsCategorySections.Interface)]
public class UiSettings : SettingsCategory
{
    public float ResolutionScale
    {
        get => GetValue(nameof(ResolutionScale), 1f);
        set => SetValue(nameof(ResolutionScale), value);
    }
    
    public bool LoadMostRecentSave
    {
        get => GetValue(nameof(LoadMostRecentSave), true);
        set => SetValue(nameof(LoadMostRecentSave), value);
    }
    
    public UiSettings(ISettingsManager settingsManager) : base(settingsManager)
    {
    }

    public override void BuildSettingsUi(ISettingsUiBuilder uiBuilder)
    {
        uiBuilder.AddSection("Accessibility", out int a11y).WithSlider("Display scale factor", "Change how large elements appear on-screen", ResolutionScale, 0.5f, 2f, v => ResolutionScale = v, a11y);
    }
}