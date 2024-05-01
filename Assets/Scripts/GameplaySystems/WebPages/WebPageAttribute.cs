using System;
using System.Collections.Generic;
using System.Web;

namespace GameplaySystems.WebPages
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class WebPageAttribute : Attribute
	{
		private readonly string[] pathParts;

		public WebPageAttribute(params string[] path)
		{
			this.pathParts = path;
		}

		public bool MatchPath(string path, Dictionary<string, string> queryParams)
		{
			string[] parts = path.Split('/');

			for (var i = 0; i < parts.Length; i++)
			{
				string match = this.pathParts[i];
				string part = parts[i];

				// This is malformed. Indicates a double slash in the path.
				if (string.IsNullOrWhiteSpace(part))
					return false;
				
				if (match.StartsWith(":"))
				{
					string key = match.Substring(1);
					queryParams[key] = HttpUtility.UrlDecode(part);
					continue;
				}

				if (part != match)
					return false;
			}

			return true;
		}
	}
}