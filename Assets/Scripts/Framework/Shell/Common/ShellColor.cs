#nullable enable

using System;

namespace Shell.Common
{
	[Serializable]
	public struct ShellColor
	{
		public ShellColorName name;
		public float r;
		public float g;
		public float b;
		public float a;
		
		private ShellColor(ShellColorName name, float r, float g, float b, float a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
			this.name = name;
		}

		public ShellColor(ShellColorName name, float a = 1)
			: this(name, 0, 0, 0, a)
		{
			
		}

		public ShellColor(float r, float g, float b, float a = 1)
			: this(ShellColorName.Custom, r, g, b, a)
		{
			
		}
	}

	[Serializable]
	public struct SimpleColor
	{
		public float r;
		public float g;
		public float b;
		public float a;
	}
}