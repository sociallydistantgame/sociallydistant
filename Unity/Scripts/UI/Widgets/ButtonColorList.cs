#nullable enable
using System;
using UnityEngine;

namespace UI.Widgets
{
	[Serializable]
	public sealed class ButtonColorList
	{
		
		private Color normalColor;

		
		private Color highlightedColor;

		
		private Color pressedColor;

		
		private Color focusedColor;

		public Color NormalColor => normalColor;
		public Color HighlightedColor => highlightedColor;
		public Color PressedColor => pressedColor;
		public Color FocusedColor => focusedColor;
	}
}