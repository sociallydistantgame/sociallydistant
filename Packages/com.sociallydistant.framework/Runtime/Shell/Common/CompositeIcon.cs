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
	}
}