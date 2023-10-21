#nullable enable
using Core.Config;

namespace UI.PlayerUI
{
	[SettingsCategory("ui", "User interface", CommonSettingsCategorySections.Interface)]
	public class UiSettings : SettingsCategory
	{
		public string ThemeName
		{
			get => GetValue(nameof(ThemeName), "default");
			set => SetValue(nameof(ThemeName), value);
		}
		
		public bool DarkMode
		{
			get => GetValue(nameof(DarkMode), false);
			set => SetValue(nameof(DarkMode), value);
		}

		/// <inheritdoc />
		public UiSettings(ISettingsManager settingsManager) : base(settingsManager)
		{
		}
	}
}