#nullable enable

using System;
using UnityEngine.Analytics;

namespace Core
{
	public static class SociallyDistantUtility
	{
		public static bool IsPosixName(string? text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return false;

			for (var i = 0; i < text.Length; i++)
			{
				char character = text[i];

				if (char.IsWhiteSpace(character))
					return false;

				if (char.IsDigit(character) && i == 0)
					return false;

				if (character == '-' && i == 0)
					return false;
				
				if (!char.IsLetterOrDigit(character) && character != '_' && character != '-')
					return false;
			}

			return true;
		}
		
		public static string GetGenderDisplayString(Gender gender)
		{
			// TODO: i18n
			return gender switch
			{
				Gender.Male => "He / Him / His",
				Gender.Female => "She / Her / Her's",
				Gender.Unknown => "They / Them / Their",
				_ => "<unknown>"
			};
		}
	}
}