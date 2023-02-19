#nullable enable

using System.Text;

namespace Utility
{
	public static class StringUtility
	{
		public static void TrimEnd(this StringBuilder stringBuilder)
		{
			while (stringBuilder.Length > 0 && char.IsWhiteSpace(stringBuilder[^1]))
			{
				stringBuilder.Length--;
			}
		}
	}
}