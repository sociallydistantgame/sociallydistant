#nullable enable
using Microsoft.Xna.Framework.Input;

namespace SociallyDistant.Core.OS.Devices
{
	public struct ConsoleInputData
	{
		public readonly Keys KeyCode;
		public readonly char Character;
		public readonly KeyModifiers Modifiers;

		public bool HasModifiers => this.Modifiers != KeyModifiers.None;
		
		public ConsoleInputData(Keys keyCode, KeyModifiers modifiers)
		{
			this.KeyCode = keyCode;
			this.Modifiers = modifiers;
			this.Character = '\0';
		}
		
		public ConsoleInputData(char character)
		{
			this.Character = character;
			this.KeyCode = Keys.None;
			this.Modifiers = KeyModifiers.None;
		}

		public ConsoleInputData(Keys keyCode)
		{
			this.Character = '\0';
			this.Modifiers =  KeyModifiers.None;
			this.KeyCode = keyCode;
		}
	}
}