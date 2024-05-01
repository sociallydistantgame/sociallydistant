#nullable enable
using System;
using UnityEngine;

namespace UI.Widgets
{
	[Serializable]
	public sealed class ButtonColors
	{
		[SerializeField]
		private ButtonColorList background = new();

		[SerializeField]
		private ButtonColorList border = new();

		public ButtonColorList Background => background;
		public ButtonColorList Border => border;
	}
}