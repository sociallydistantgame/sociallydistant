#nullable enable

using System.Text;

namespace SociallyDistant.Core.Core
{
	public static class StringUtility
	{
		public static byte GetHexDigitValue(char hex)
		{
			return hex switch
			{
				'0' => 0,
				'1' => 1,
				'2' => 2,
				'3' => 3,
				'4' => 4,
				'5' => 5,
				'6' => 6,
				'7' => 7,
				'8' => 8,
				'9' => 9,
				'a' => 10,
				'A' => 10,
				'b' => 11,
				'B' => 11,
				'c' => 12,
				'C' => 12,
				'd' => 13,
				'D' => 13,
				'e' => 14,
				'E' => 14,
				'f' => 15,
				'F' => 15,
				_ => throw new FormatException($"{hex} is not a hex digit")
			};
		}

		public static string StripNewLines(this string text)
		{
			return text.Replace("\r", string.Empty)
				.Replace("\n", string.Empty);
		}
		
		public static void TrimEnd(this StringBuilder stringBuilder)
		{
			while (stringBuilder.Length > 0 && char.IsWhiteSpace(stringBuilder[^1]))
			{
				stringBuilder.Length--;
			}
		}
	}
}