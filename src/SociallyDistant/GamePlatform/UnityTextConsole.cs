#nullable enable
using System.Diagnostics;
using Serilog;
using SociallyDistant.Core.OS.Devices;

namespace SociallyDistant.GamePlatform
{
	public class UnityTextConsole : ITextConsole
	{
		/// <inheritdoc />
		public string WindowTitle { get; set; }

		/// <inheritdoc />
		public bool IsInteractive => false;

		/// <inheritdoc />
		public void ClearScreen()
		{
			// Stub. Can't clear the Unity console.
		}

		/// <inheritdoc />
		public void WriteText(string text)
		{
			Log.Information($"[Shell script message] {text}");
		}

		/// <inheritdoc />
		public ConsoleInputData? ReadInput()
		{
			Log.Error("Reading keyboard input from the Unity console is not supported.");
			return null;
		}
	}
}