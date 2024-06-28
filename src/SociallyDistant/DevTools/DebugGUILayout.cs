#nullable enable
using ImGuiNET;

namespace SociallyDistant.DevTools
{
	public static class DebugGUILayout
	{
		public static byte ByteField(byte value)
		{
			int byteValue = value;
			if (ImGui.InputInt(string.Empty, ref byteValue))
				return (byte)byteValue;
			
			return value;
		}
		
		public static ushort UShortField(ushort value)
		{
			int shortValue = value;
			if (ImGui.InputInt(string.Empty, ref shortValue))
				return (ushort)shortValue;
			
			return value;
		}
	}
}