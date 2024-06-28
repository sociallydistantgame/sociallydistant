using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AcidicGui.Widgets
{
	public class ButtonWidgetController : WidgetController
	{
		
		private Button button = null!;

		
		private TextMeshProUGUI text = null!;
		
		public string Text { get; set; } = string.Empty;
		public Action? ClickAction { get; set; }
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			text.SetText(Text);
			button.onClick.AddListener(OnClick);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			ClickAction = null;
			button.onClick.RemoveAllListeners();
		}

		private void OnClick()
		{
			ClickAction?.Invoke();
		}
	}
}