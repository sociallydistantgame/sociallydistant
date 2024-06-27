#nullable enable
using Core.Config;
using UnityEngine;

namespace Audio
{
	[SettingsCategory("audio", "Audio", CommonSettingsCategorySections.Hardware)]
	public class AudioSettings : SettingsCategory
	{
		public float SfxVolume
		{
			get => GetValue(nameof(SfxVolume), 1);
			set => SetValue(nameof(SfxVolume), Mathf.Clamp(value, 0, 1));
		}

		public float MusicVolume
		{
			get => GetValue(nameof(MusicVolume), 1);
			set => SetValue(nameof(MusicVolume), Mathf.Clamp(value, 0, 1));
		}

		public bool TypingSounds
		{
			get => GetValue(nameof(TypingSounds), true);
			set => SetValue(nameof(TypingSounds), value);
		}

		public bool AudibleBell
		{
			get => GetValue(nameof(AudibleBell), true);
			set => SetValue(nameof(AudibleBell), value);
		}

		public bool TerminalOutputEffects
		{
			get => GetValue(nameof(TerminalOutputEffects), true);
			set => SetValue(nameof(TerminalOutputEffects), value);
		}
		
	/// <inheritdoc />
		public AudioSettings(ISettingsManager settingsManager) : base(settingsManager)
		{ }

		/// <inheritdoc />
		public override void BuildSettingsUi(ISettingsUiBuilder uiBuilder)
		{
			uiBuilder.AddSection(CommonSettingsSections.Volume, out int volume)
				.WithSlider(
					"UI sounds",
					null,
					this.SfxVolume,
					0,
					1,
					x => SfxVolume = x,
					volume
				).WithSlider(
					"Background music",
					null,
					MusicVolume,
					0,
					1,
					x => MusicVolume = x,
					volume
				);

			uiBuilder.AddSection("Effects", out int effectsSection)
				.WithToggle(
					"Typing sounds",
					"Play sounds when typing in the Terminal and other input fields",
					TypingSounds,
					x => TypingSounds = x,
					effectsSection
				).WithToggle(
					"Audible Bells",
					"Play an audible beep sound when an action is blocked by the Terminal.",
					AudibleBell,
					x => AudibleBell = x,
					effectsSection
				).WithToggle(
					"Terminal Output Effects",
					"Play various indicator sounds when commands print text to the Terminal.",
					TerminalOutputEffects,
					x => TerminalOutputEffects = x,
					effectsSection
				);

		}
	}
}