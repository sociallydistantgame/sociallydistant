#nullable enable

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Widgets
{
	public class ButtonWidget : MonoBehaviour
	{
		private Button button = null!;

		[SerializeField]
		private TextMeshProUGUI label = null!;

		public string Text
		{
			get => label.text;
			set => label.text = value;
		}

		public event Action<ButtonWidget>? Clicked; 

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