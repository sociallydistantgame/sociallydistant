#nullable enable

using System;
using UnityEngine;

namespace Shell.Common
{
	[Serializable]
	public struct CompositeIcon
	{
		public string textIcon;
		public Sprite? spriteIcon;
		public ShellColor iconColor;
		
		public static implicit operator CompositeIcon(string unicodeTextIcon)
		{
			return new CompositeIcon
			{
				textIcon = unicodeTextIcon,
				spriteIcon = null,
				iconColor = new ShellColor(1, 1, 1, 1)
			};
		}
	}
}