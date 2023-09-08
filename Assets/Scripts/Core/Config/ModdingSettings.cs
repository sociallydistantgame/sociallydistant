namespace Core.Config
{
	[SettingsCategory("com.sociallydistant.modding", "Mod settings")]
	[SystemSettings("Mods")]
	public class ModdingSettings : SettingsCategory
	{
		[SettingsField("Enable script mods", "Allow execution of script mods. By turning this on, you acknowledge that Socially Distant and its development team is not responsible for the content and behaviour of any installed mods, and that we accept no responsibility for any damage done to the game or your computer.", CommonSettingsSections.Legal)]
		public bool AcceptLegalWaiver
		{
			get => GetValue(nameof(AcceptLegalWaiver), false);
			set => SetValue(nameof(AcceptLegalWaiver), value);
		}

		/// <inheritdoc />
		public ModdingSettings(ISettingsManager settingsManager) : base(settingsManager)
		{ }
	}
}