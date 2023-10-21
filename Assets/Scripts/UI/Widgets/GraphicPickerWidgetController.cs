#nullable enable
using System;
using AcidicGui.Widgets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Widgets
{
	public class GraphicPickerWidgetController : WidgetController
	{
		[Header("UI")]
		[SerializeField]
		private TextMeshProUGUI graphicName = null!;

		[SerializeField]
		private RawImage graphic = null!;

		[SerializeField]
		private Button replaceButton = null!;

		[SerializeField]
		private Button removeButton = null!;

		private AspectRatioFitter aspectRatioFitter = null!;
		
		public string? GraphicName { get; set; }
		public Action<string?, Texture2D?>? Callback { get; set; }
		public IGraphicPickerSource? GraphicSource { get; set; }
		
		private void Awake()
		{
			this.graphic.MustGetComponent(out aspectRatioFitter);
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
		}
	}
}