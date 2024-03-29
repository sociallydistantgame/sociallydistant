#nullable enable
using Core.Config;
using Core.Config.SystemConfigCategories;
using UI.Widgets.Settings;
using UnityEngine.Rendering.PostProcessing;
using UnityExtensions;

namespace UI.PlayerUI
{
	public sealed class PostProcessSettingsListener : SettingsListener
	{
		private PostProcessVolume volume = null!;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.MustGetComponent(out volume);
		}

		/// <inheritdoc />
		protected override void OnSettingsChanged(ISettingsManager settingsManager)
		{
			var graphicsSettings = new GraphicsSettings(settingsManager);

			if (volume.profile.TryGetSettings(out Bloom bloom))
			{
				bloom.enabled.value = graphicsSettings.BloomEffect;
			}
			
		}
	}
}