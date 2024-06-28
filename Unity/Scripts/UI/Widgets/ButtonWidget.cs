#nullable enable

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.Widgets
{
	public class ButtonWidget : MonoBehaviour
	{
		
		private TextMeshProUGUI label = null!;

		private Button button = null!;
		
		public string Text
		{
			get => label.text;
			set => label.SetText(value);
		}

		public Action<ButtonWidget>? Clicked { get; set; }

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ButtonWidget));
			this.MustGetComponent(out button);
			
			this.button.onClick.AddListener(HandleClick);
		}

		private void HandleClick()
		{
			Clicked?.Invoke(this);
		}
	}
}