#nullable enable
using Core.Config;
using UI.Widgets.Settings;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.PlayerUI
{
	public sealed class GuiSettingsListener : SettingsListener
	{
		[SerializeField]
		private GuiLayer visualLayer;

		[Range(-500, 500)]
		[SerializeField]
		private int layerOffset = 0;
		
		private Canvas canvas;
		private CanvasScaler canvasScaler;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.MustGetComponent(out canvas);
			this.MustGetComponent(out canvasScaler);
			
			canvas.worldCamera = Camera.main;
			canvas.renderMode = RenderMode.ScreenSpaceCamera;

			int guiLayer = ((int) this.visualLayer + 500) + layerOffset;
			this.canvas.planeDistance = 6000 - guiLayer + 1;
		}

		/// <inheritdoc />
		protected override void OnSettingsChanged(ISettingsManager settingsManager)
		{
		}

		private enum GuiLayer
		{
			Backdrop,
			Windows,
			Shell,
			Popovers,
			Overlay
		}
	}
}