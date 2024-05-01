#nullable enable
using System;
using System.Diagnostics;
using Shell;
using Shell.Common;
using TMPro;
using UnityEngine;
using UnityExtensions;
using Utility;
using UnityEngine.UI;

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
		private CompositeIconWidget icon = null!;

		private Button baseButton = null!;
		
		public CompositeIcon Icon
		{
			get => icon.Icon;
			set => icon.Icon = value;
		}
		
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
			this.MustGetComponent(out baseButton);
		}

		private void Start()
		{
			Color accentColor = CommonColor.Blue.GetColor();
			
			var colorBlock = new ColorBlock();
			colorBlock.normalColor = default;
			colorBlock.selectedColor = accentColor;
			colorBlock.highlightedColor = accentColor.AlphaAdjust(0.75f);
			colorBlock.pressedColor = accentColor.AlphaAdjust(0.5f);
			colorBlock.colorMultiplier = 1;
			colorBlock.fadeDuration = 0.1f;
			
			baseButton.colors = colorBlock;
		}
	}
}