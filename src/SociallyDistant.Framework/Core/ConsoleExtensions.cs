#nullable enable
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.Core.Core
{
	public static class ConsoleExtensions
	{
		public static void SetCursorPos(this ConsoleWrapper console, int x, int y)
		{
			console.Write($"\x1b[{y + 1};{x + 1}H");
		}

		public static void UseAltScreen(this ConsoleWrapper console, bool value)
		{
			char set = value ? 'h' : 'l';
			console.Write($"\x1b[1049{set}");
		}
		
		public static void WriteWithTimestamp(this ConsoleWrapper console, string text)
		{
			var totalSpace = 12;
			
			float time = Time.time;
			string timestamp = time.ToString("0.000000");

			var spaceNeeded = totalSpace - timestamp.Length;
			
			console.Write("[");
			
			for (var i = 0; i < spaceNeeded; i++)
				console.Write(" ");
			
			console.Write(timestamp);
			console.Write("] ");

			console.WriteLine(text);
		}
	}
}