using System;
using UI.Themes.Serialization;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class ThemeColor : IThemeData
	{
		[SerializeField]
		private int type;

		[SerializeField]
		private string value = string.Empty;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(ref type, nameof(type), (int) ThemeColorType.Custom);
			serializer.Serialize(ref value, nameof(value), "#00000000");
		}
	}
}