#nullable enable
using OS.Devices;
using UnityEngine;

namespace GamePlatform
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
			Debug.Log($"[Shell script message] {text}");
		}

		/// <inheritdoc />
		public ConsoleInputData? ReadInput()
		{
			Debug.LogError("Reading keyboard input from the Unity console is not supported.");
			return null;
		}
	}
}