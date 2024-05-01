#nullable enable
using System;
using AcidicGui.Mvc;
using AcidicGui.Widgets;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UI.Widgets.Settings
{
	public class SliderWidgetController : WidgetController
	{
		[SerializeField]
		private Slider slider = null!;
		
		public Action<float>? Callback { get; set; }
		public float Value { get; set; }
		public float MinValue { get; set; }
		public float MaxValue { get; set; }

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(SliderWidgetController));
		}

		private void Start()
		{
			slider.onValueChanged.AddListener(OnValueChanged);
		}

		/// <inheritdoc />
		public override void UpdateUI()
		{
			slider.minValue = this.MinValue;
			slider.maxValue = this.MaxValue;
			slider.SetValueWithoutNotify(this.Value);
		}

		/// <inheritdoc />
		public override void OnRecycle()
		{
			Callback = null;
		}

		private void OnValueChanged(float newValue)
		{
			this.Callback?.Invoke(newValue);
		}
	}
}