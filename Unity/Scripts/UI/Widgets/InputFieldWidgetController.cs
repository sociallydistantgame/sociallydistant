#nullable enable
using System;
using AcidicGui.Widgets;
using Core.Config;
using GamePlatform;
using ThisOtherThing.UI.Shapes;
using TMPro;
using TrixelCreative.TrixelAudio;
using TrixelCreative.TrixelAudio.Data;
using UnityEngine;
using UnityExtensions;
using AudioSettings = Audio.AudioSettings;

namespace UI.Widgets
{
	public sealed class InputFieldWidgetController : WidgetController
	{
		[SerializeField]
		private Rectangle border = null!;
		
		[SerializeField]
		private TMP_InputField inputField = null!;

		[SerializeField]
		private TextMeshProUGUI placeholder = null!;

		[SerializeField]
		private InputFieldAnimationDriver borderHighlight = null!;
		
		public bool UseFullBorder { get; set; }
		public string Value { get; set; } = string.Empty;
		public string Placeholder { get; set; } = string.Empty;
		public Action<string>? OnValueChanged { get; set; }
		
		/// <inheritdoc />
		public override void UpdateUI()
		{
			this.borderHighlight.UseFullBorder = UseFullBorder;
			
			placeholder.SetText(Placeholder);
			inputField.SetTextWithoutNotify(Value);
			
			this.inputField.onValueChanged.AddListener(OnInputValueChanged);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			this.inputField.onValueChanged.RemoveListener(OnInputValueChanged);
			OnValueChanged = null;
		}
		
		private void OnInputValueChanged(string newValue)
		{
			OnValueChanged?.Invoke(newValue);
		}
	}
}