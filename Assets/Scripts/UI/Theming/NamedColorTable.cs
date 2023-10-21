#nullable enable

using System;
using Architecture;
using UI.Themes.ThemeData;
using UnityEngine;

namespace UI.Theming
{
	[Serializable]
	public class NamedColorTable : SerializableDictionary<string, NamedColor>
	{
		public Color TranslateColor(ThemeColor color, bool dark)
		{
			Color result = default;
			
			if (color.IsCustom)
			{
				string hex = color.Data;
				if (!color.Data.StartsWith("#"))
					hex = "#" + color.Data;

				ColorUtility.TryParseHtmlString(hex, out result);
			}
			else
			{
				if (this.TryGetValue(color.Data, out NamedColor namedColor))
				{
					result = dark ? namedColor.darkColor : namedColor.lightColor;
				}
			}

			return result;
		}
	}
}