#nullable enable
using System;
using AcidicGui.Widgets;
using Player;
using Shell.Windowing;
using TMPro;
using UI.Widgets.Prefabs.WidgetWindows;
using UI.Windowing;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Widgets
{
	public class GraphicPickerWidgetController : WidgetController
	{
		[Header("Dependencies")]
		[SerializeField]
		private PlayerInstanceHolder playerInstanceHolder = null!;
		
		[Header("UI")]
		[SerializeField]
		private TextMeshProUGUI graphicName = null!;

		[SerializeField]
		private RawImage graphic = null!;

		[SerializeField]
		private Button replaceButton = null!;

		[SerializeField]
		private Button removeButton = null!;

		[Header("Windows")]
		[SerializeField]
		private GraphicChooserWindow windowPrefab = null!;
		
		
		private AspectRatioFitter aspectRatioFitter = null!;
		
		public string? GraphicName { get; set; }
		public Action<string?, Texture2D?>? Callback { get; set; }
		public IGraphicPickerSource? GraphicSource { get; set; }
		
		private void Awake()
		{
			
			this.graphic.MustGetComponent(out aspectRatioFitter);
		}

		private void Start()
		{
			this.replaceButton.onClick.AddListener(OnReplaceClicked);
			this.removeButton.onClick.AddListener(OnRemoveClicked);
		}

		/// <inheritdoc />
		public override void UpdateUI()
		{
			if (string.IsNullOrWhiteSpace(GraphicName))
			{
				this.graphic.texture = null;
				this.graphic.color = default;
				this.graphicName.SetText("No graphic selected");
				this.removeButton.gameObject.SetActive(false);
			}
			else
			{
				Texture2D? texture = GraphicSource?.GetGraphic(GraphicName);
				graphic.texture = texture;
				graphic.color = texture != null ? Color.white : default;
				graphicName.SetText(graphic.texture == null
					? $"{GraphicName} (missing)"
					: GraphicName);

				if (texture != null)
				{
					aspectRatioFitter.aspectRatio = (float) texture.width / (float) texture.height;
				}
				
				this.removeButton.gameObject.SetActive(true);
			}
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			Callback = null;
			GraphicSource = null;
			GraphicName = null;
			graphic.texture = null;
		}

		private async void OnReplaceClicked()
		{
			replaceButton.enabled = false;

			OverlayWorkspace overlay = playerInstanceHolder.Value.UiManager.WindowManager.CreateSystemOverlay();

			IWindow win = overlay.CreateWindow("Graphic Picker - Choose Graphic");

			if (win is not UguiWindow guiWin || GraphicSource == null)
			{
				replaceButton.enabled = true;
				win.Close();
				return;
			}

			GraphicChooserWindow chooser = Instantiate(windowPrefab, guiWin.ClientArea);
            
			GraphicName = await chooser.GetNewGraphic(GraphicName, GraphicSource);

			UpdateUI();
			
			Callback?.Invoke(GraphicName, graphic.texture as Texture2D);

			win.ForceClose();
			
			replaceButton.enabled = true;
		}

		private void OnRemoveClicked()
		{
			GraphicName = string.Empty;
			UpdateUI();
			
			Callback?.Invoke(GraphicName, graphic.texture as Texture2D);
		}
	}
}