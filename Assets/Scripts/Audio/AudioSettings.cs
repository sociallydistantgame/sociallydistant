#nullable enable
using Core.Config;
using UnityEngine;

namespace Audio
{
	[SystemSettings("Hardware")]
	[SettingsCategory("audio", "Audio")]
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
		}
	}
}