using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AcidicGui.Widgets
{
	public class ListItemWidgetController : WidgetController
	{
		
		private RectTransform slot = null!;
		
		
		private Toggle toggle;

		
		private TextMeshProUGUI label = null!;

		
		private AnimatedHighlight animatedHighlight = null!;
		
		
		private TextMeshProUGUI description = null!;

		private bool ignoreCallback;

		public RectTransform ImageSlot => slot;
		
		public bool Selected { get; set; }
		public Action? Callback { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public WidgetController? ImageWidget { get; set; }
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			animatedHighlight.IsActive = Selected;
			
			if (string.IsNullOrWhiteSpace(Description))
			{
				label.SetText(string.Empty);
				description.SetText(Title);
			}
			else
			{
				label.SetText(Title);
				description.SetText(Description);
			}
			
			toggle.SetIsOnWithoutNotify(Selected);
			toggle.onValueChanged.AddListener(OnValueChanged);
			
			if (this.ImageWidget != null)
			{
				this.ImageWidget.UpdateUI();
				this.ImageWidget.gameObject.SetActive(true);
			}
		}
		
		private void OnValueChanged(bool newValue)
		{
			if (!newValue)
				return;

			Callback?.Invoke();
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			if (ImageWidget != null)
			{
				ImageWidget.gameObject.SetActive(false);
				ImageWidget.OnRecycle();
			}
			
			animatedHighlight.IsActive = false;
			
			if (ignoreCallback)
				return;
			
			ignoreCallback = true;
			
			toggle.onValueChanged.RemoveAllListeners();

			toggle.group = null;
			toggle.group = null;
			Callback = null;
			
			ignoreCallback = false;
		}
	}
}