#nullable enable

using System.Collections.Generic;
using System.Text;

namespace Core
{
	public static class PathUtility
	{
		public static char SeparatorChar = '/';

		public static string Combine(params string[] paths)
		{
			var sb = new StringBuilder();

			foreach (string part in paths)
			{
				if (sb.Length > 0 && sb[^1] != SeparatorChar)
					sb.Append(SeparatorChar);
				
				if (part.StartsWith(SeparatorChar))
					sb.Length = 0;

				sb.Append(part);
			}
			
			return sb.ToString();
		}

		public static string[] Split(string path)
		{
			var sb = new StringBuilder();
			var parts = new List<string>();

			for (var i = 0; i <= path.Length; i++)
			{
				if (i == path.Length)
				{
					if (sb.Length > 0)
						parts.Add(sb.ToString());
					sb.Length = 0;
					break;
				}
				
				char character = path[i];

				if (character == SeparatorChar)
				{
					if (sb.Length > 0)
						parts.Add(sb.ToString());
					sb.Length = 0;
					continue;
				}

				sb.Append(character);
			}

			return parts.ToArray();
		}

		public static string GetFileName(string path)
		{
			// String is empty or last character is a separator, there's no file name
			if (path.Length == 0 || path[^1] == SeparatorChar)
				return string.Empty;
			
			int lastSeparator = path.LastIndexOf(SeparatorChar);

			// No separator? It's just a filename.
			if (lastSeparator == -1)
				return path;

			return path.Substring(lastSeparator + 1);
		}

		public static string GetDirectoryName(string path)
		{
			// Path is empty, return a single separator
			if (path.Length == 0)
				return SeparatorChar.ToString();
			
			// Path ends with a separator, entire path is a directory
			if (path[^1] == SeparatorChar)
				return path;
			
			// Find last separator
			int lastSeparator = path.LastIndexOf(SeparatorChar);
			
			// No separators, not a path.
			if (lastSeparator == -1)
				return string.Empty;

			string parent = path.Substring(0, lastSeparator);
			if (parent.Length == 0)
				return SeparatorChar.ToString();
			return parent;
		}
	}
}