#nullable enable
using System;

namespace OS.Devices
{
	[Flags]
	public enum KeyModifiers
	{
		None = 0,
		Control = 1,
		Alt = 2,
		Shift = 4
	}
}