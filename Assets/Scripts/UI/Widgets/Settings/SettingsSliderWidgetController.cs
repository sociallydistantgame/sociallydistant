#nullable enable

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Widgets.Settings
{
	public class SettingsSliderWidgetController : SettingsWidgetController<SettingsSliderWidget>
	{
		[SerializeField]
		private TextMeshProUGUI titleText = null!;

		[SerializeField]
		private TextMeshProUGUI descriptionText = null!;

		[SerializeField]
		private Slider slider = null!;

		private string title = string.Empty;
		private string? description;
		private float minValue;
		private float maxValue;
		private float value;
		private Action<float>? callback;

		/// <inheritdoc />
		public override void UpdateUI()
		{
			title = Widget.Label;
			description = Widget.Description;
			minValue = Widget.MinimumValue;
			maxValue = Widget.MaximumValue;
			value = Widget.Value;
			callback = Widget.Callback;
			
			titleText.SetText(title);
			descriptionText.SetText(description);

			slider.minValue = minValue;
			slider.maxValue = maxValue;
			slider.value = value;
			
			slider.onValueChanged.AddListener(OnValueChanged);
		}

		private void OnValueChanged(float newValue)
		{
			value = newValue;
			callback?.Invoke(newValue);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			callback = null;
			slider.onValueChanged.RemoveAllListeners();
		}
	}
}