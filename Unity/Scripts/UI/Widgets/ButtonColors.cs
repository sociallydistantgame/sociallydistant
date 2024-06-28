#nullable enable
using System;
using UnityEngine;

namespace UI.Widgets
{
	[Serializable]
	public sealed class ButtonColors
	{
		
		private ButtonColorList background = new();

		
		private ButtonColorList border = new();

		public ButtonColorList Background => background;
		public ButtonColorList Border => border;
	}
}