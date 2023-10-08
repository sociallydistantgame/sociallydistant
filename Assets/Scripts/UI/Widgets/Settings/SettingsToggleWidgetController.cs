using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Widgets.Settings
{
	public class SettingsToggleWidgetController : SettingsWidgetController<SettingsToggleWidget>
	{
		[SerializeField]
		private TextMeshProUGUI titleText = null!;

		[SerializeField]
		private TextMeshProUGUI descriptionText = null!;

		[SerializeField]
		private Toggle actualToggle = null!;
		
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public bool CurrentValue { get; set; }
		public Action<bool>? Callback { get; set; }

		/// <inheritdoc />
		public override void UpdateUI()
		{
			this.Title = Widget.Title;
			this.Description = Widget.Description;
			this.CurrentValue = Widget.CurrentValue;
			this.Callback = Widget.Callback;
			
			this.titleText.SetText(this.Title);
			this.descriptionText.SetText(this.Description);
			this.actualToggle.isOn = this.CurrentValue;
			
			this.actualToggle.onValueChanged.AddListener(OnValueChanged);
		}

		private void OnValueChanged(bool newValue)
		{
			this.CurrentValue = newValue;
			this.Callback?.Invoke(this.CurrentValue);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			this.actualToggle.onValueChanged.RemoveAllListeners();
		}
	}
}