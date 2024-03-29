#nullable enable
using System.Net.Mime;
using Core.Config;
using Core.Config.SystemConfigCategories;
using Shell.Windowing;
using UI.Widgets.Settings;
using UI.Windowing;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Applications.Terminal
{
	public sealed class TerminalSettingsListener : SettingsListener
	{
		private WindowHintProvider hintProvider;
		private Graphic blurGraphic;
		private IWindow parentWindow;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.MustGetComponentInParent(out parentWindow);
			this.MustGetComponent(out blurGraphic);
			this.MustGetComponent(out hintProvider);
			
		}

		/// <inheritdoc />
		protected override void OnSettingsChanged(ISettingsManager settingsManager)
		{
			var accessibilitySettings = new AccessibilitySettings(settingsManager);

			blurGraphic.enabled = !accessibilitySettings.DisableTerminalBackgroundBlur;

			WindowHints hints = hintProvider.Hints;
			hints.ClientRendersWindowBackground = blurGraphic.enabled;
			parentWindow.SetWindowHints(hints);
			
			hintProvider.Hints = hints;
			
		}
	}
}