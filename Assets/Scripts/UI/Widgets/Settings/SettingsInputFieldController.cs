using System;
using TMPro;
using UnityEngine;

namespace UI.Widgets.Settings
{
	public class SettingsInputFieldController : SettingsWidgetController<SettingsInputFieldWidget>
	{
		[SerializeField]
		private TextMeshProUGUI titleText = null!;

		[SerializeField]
		private TextMeshProUGUI descriptionText = null!;

		[SerializeField]
		private TMP_InputField inputField = null!;
		
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string? CurrentValue { get; set; }
		public Action<string>? Callback { get; set; }
		
		/// <inheritdoc />
		public override void Setup(SettingsInputFieldWidget widget)
		{
			this.Title = widget.Title;
			this.Description = widget.Description;
			this.CurrentValue = widget.CurrentValue;
			this.Callback = widget.Callback;
		}

		/// <inheritdoc />
		public override void UpdateUI()
		{
			titleText.SetText(Title);
			descriptionText.SetText(Description);
			inputField.text = CurrentValue;
			inputField.onValueChanged.AddListener(OnValueChanged);
		}

		private void OnValueChanged(string newValue)
		{
			this.CurrentValue = newValue;
			Callback?.Invoke(newValue);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			this.inputField.onValueChanged.RemoveAllListeners();
		}
	}
}