#nullable enable
using UnityEngine;

namespace DevTools
{
	public static class DebugGUILayout
	{
		public static byte ByteField(byte value)
		{
			if (byte.TryParse(GUILayout.TextField(value.ToString()), out byte newValue))
				return newValue;
			
			return value;
		}
		
		public static ushort UShortField(ushort value)
		{
			if (ushort.TryParse(GUILayout.TextField(value.ToString()), out ushort newValue))
				return newValue;
			
			return value;
		}
	}
}