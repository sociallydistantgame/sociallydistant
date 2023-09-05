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
        
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(TwoLineButtonWithIcon));
		}
	}
}