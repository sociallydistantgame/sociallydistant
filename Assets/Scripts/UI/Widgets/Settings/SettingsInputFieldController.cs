#nullable enable

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
		public override void UpdateUI()
		{
			Title = Widget.Title;
			Description = Widget.Description;
			CurrentValue = Widget.CurrentValue;
			Callback = Widget.Callback;
			
			titleText.SetText(Title);
			descriptionText.SetText(Description);
			inputField.SetTextWithoutNotify(CurrentValue);
			inputField.onValueChanged.AddListener(OnValueChanged);
		}

		private void OnValueChanged(string newValue)
		{
			this.CurrentValue = newValue;
			Widget.CurrentValue = newValue;
			Callback?.Invoke(newValue);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			this.inputField.onValueChanged.RemoveAllListeners();
		}
	}
}