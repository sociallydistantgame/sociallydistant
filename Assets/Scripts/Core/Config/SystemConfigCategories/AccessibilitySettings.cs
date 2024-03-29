#nullable enable

using UI.SystemSettings;

namespace Core.Config.SystemConfigCategories
{
	[SettingsCategory("a11y", "Accessibility", CommonSettingsCategorySections.Interface)]
	public class AccessibilitySettings : SettingsCategory
	{
		public bool UseHyperlegibleFont
		{
			get => GetValue(nameof(UseHyperlegibleFont), false);
			set => SetValue(nameof(UseHyperlegibleFont), value);
		}

		public bool UseHighContrastTerminalColors
		{
			get => GetValue(nameof(UseHighContrastTerminalColors), false);
			set => SetValue(nameof(UseHighContrastTerminalColors), value);
		}
		
		public bool ShowImageCaptions
		{
			get => GetValue(nameof(ShowImageCaptions), false);
			set => SetValue(nameof(ShowImageCaptions), value);
		}
        
		public bool ForceWebsiteDarkMode
		{
			get => GetValue(nameof(ForceWebsiteDarkMode), false);
			set => SetValue(nameof(ForceWebsiteDarkMode), value);
		}
		
		public bool UseOpaqueInfoPanel
		{
			get => GetValue(nameof(UseOpaqueInfoPanel), false);
			set => SetValue(nameof(UseOpaqueInfoPanel), value);
		}
		
		public bool ShowChatAuthorNames
		{
			get => GetValue(nameof(ShowChatAuthorNames), false);
			set => SetValue(nameof(ShowChatAuthorNames), value);
		}

		public bool UseChatBubbleOutlines
		{
			get => GetValue(nameof(UseChatBubbleOutlines), false);
			set => SetValue(nameof(UseChatBubbleOutlines), value);
		}

		public bool TranscribeVoiceRecordings
		{
			get => GetValue(nameof(TranscribeVoiceRecordings), false);
			set => SetValue(nameof(TranscribeVoiceRecordings), value);
		}

		public bool SaveAudioTranscripts
		{
			get => GetValue(nameof(SaveAudioTranscripts), false);
			set => SetValue(nameof(SaveAudioTranscripts), value);
		}

		public bool DisableTerminalBackgroundBlur
		{
			get => GetValue(nameof(DisableTerminalBackgroundBlur), false);
			set => SetValue(nameof(DisableTerminalBackgroundBlur), value);
		}
		
		/// <inheritdoc />
		public AccessibilitySettings(ISettingsManager settingsManager) : base(settingsManager)
		{
		}

		/// <inheritdoc />
		public override void BuildSettingsUi(ISettingsUiBuilder uiBuilder)
		{
			uiBuilder.AddSection("Vision and reading", out int vision)
				.WithToggle(
					"Use high-contrast terminal colors",
					"If enabled, the Terminal will use a high-contrast color scheme instead of the color scheme defined in the current UI theme.",
					UseHighContrastTerminalColors,
					x => UseHighContrastTerminalColors = x,
					vision
                )
				.WithToggle(
					"Use hyperlegible font",
					"Replace the default font family of UI elements with a more legible font family designed for users with dyslexia and/or visual impairment.",
					UseHyperlegibleFont,
					x => UseHyperlegibleFont = x,
					vision
                )
				.WithToggle(
					"Disable terminal blur",
					"Disable the translucent background blur of the Terminal.",
					DisableTerminalBackgroundBlur,
					x => DisableTerminalBackgroundBlur = x,
					vision
				)
				.WithToggle(
					"Opaque Info Panel",
					"Use an opaque background color for the desktop's Info Panel. This may improve the contrast of Info Panel text against the desktop background.",
					UseOpaqueInfoPanel,
					x => UseOpaqueInfoPanel = x,
					vision
				)
				.WithToggle(
					"Show chat message author names",
					"Show the names and usernames of chat message authors above the message contents. Normally, only the user's avatar is shown next to the message contents.",
					ShowChatAuthorNames,
					x => ShowChatAuthorNames = x,
					vision
				).WithToggle(
					"Only use chat bubble outlines",
					"Normally, chat bubbles are filled with a color to identify whether they're from you or an NPC. Enabling this setting will use a colored outline instead, which may improve readability of the message's text.",
					UseChatBubbleOutlines,
					x => UseChatBubbleOutlines = x,
					vision
				).WithToggle(
					"Force dark theme for all websites",
					"Normally, websites use a unique UI theme. If this setting is enabled, websites will instead be rendered using the Socially Distant OS dark theme.",
					ForceWebsiteDarkMode,
					x => ForceWebsiteDarkMode = x,
					vision
				).WithToggle(
					"Show text captions for images",
					"Show text-based descriptions of images in emails, social posts, chat messages, and on web pages.",
					ShowImageCaptions,
					x => ShowImageCaptions = x,
					vision
				);

			uiBuilder.AddSection("Hearing", out int hearing)
				.WithToggle(
					"Transcribe voice recordings",
					"When listening to recorded voice clips, and when in voice calls with NPCs, show a live text transcript of the conversation.",
					TranscribeVoiceRecordings,
					x => TranscribeVoiceRecordings = x,
					hearing
				).WithToggle(
					"Save transcripts as chat conversations",
					"If enabled, audio transcripts will be saved as chat conversations to be reviewed later.",
					SaveAudioTranscripts,
					x => SaveAudioTranscripts = x,
					hearing
				);
		}
	}
}