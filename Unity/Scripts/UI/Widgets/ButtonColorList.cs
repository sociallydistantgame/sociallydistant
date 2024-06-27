#nullable enable
using System;
using UnityEngine;

namespace UI.Widgets
{
	[Serializable]
	public sealed class ButtonColorList
	{
		[SerializeField]
		private Color normalColor;

		[SerializeField]
		private Color highlightedColor;

		[SerializeField]
		private Color pressedColor;

		[SerializeField]
		private Color focusedColor;

		public Color NormalColor => normalColor;
		public Color HighlightedColor => highlightedColor;
		public Color PressedColor => pressedColor;
		public Color FocusedColor => focusedColor;
	}
}