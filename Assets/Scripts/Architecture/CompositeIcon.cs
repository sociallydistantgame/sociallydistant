#nullable enable

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Architecture
{
	[Serializable]
	public struct CompositeIcon
	{
		public string textIcon;
		public Sprite? spriteIcon;
		public Color iconColor;
	}
}