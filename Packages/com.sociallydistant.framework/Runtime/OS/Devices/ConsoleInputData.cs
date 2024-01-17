#nullable enable
using UnityEngine;

namespace OS.Devices
{
	public struct ConsoleInputData
	{
		public readonly KeyCode KeyCode;
		public readonly char Character;
		public readonly KeyModifiers Modifiers;

		public bool HasModifiers => this.Modifiers != KeyModifiers.None;
		
		public ConsoleInputData(KeyCode keyCode, KeyModifiers modifiers)
		{
			this.KeyCode = keyCode;
			this.Modifiers = modifiers;
			this.Character = '\0';
		}
		
		public ConsoleInputData(char character)
		{
			this.Character = character;
			this.KeyCode = KeyCode.None;
			this.Modifiers = KeyModifiers.None;
		}

		public ConsoleInputData(KeyCode keyCode)
		{
			this.Character = '\0';
			this.Modifiers =  KeyModifiers.None;
			this.KeyCode = keyCode;
		}
	}
}