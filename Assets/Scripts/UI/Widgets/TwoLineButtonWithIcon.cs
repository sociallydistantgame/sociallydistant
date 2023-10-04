#nullable enable
using System;
using TMPro;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace UI.Widgets
{
	public class TwoLineButtonWithIcon : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField]
		private TextMeshProUGUI title = null!;
		
		[SerializeField]
		private TextMeshProUGUI text = null!;
		
		[SerializeField]
		private TextMeshProUGUI icon = null!;

		public string FirstLine
		{
			get => title.text;
			set => title.SetText(value);
		}

		public string SecondLine
		{
			get => text.text;
			set => text.SetText(value);
		}
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TwoLineButtonWithIcon));
		}
	}
}