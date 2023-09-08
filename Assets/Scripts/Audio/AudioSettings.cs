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
	}
}