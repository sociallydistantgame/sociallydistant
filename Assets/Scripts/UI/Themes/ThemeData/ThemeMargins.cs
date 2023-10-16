using System;
using UI.Themes.Serialization;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public struct ThemeMargins : IThemeData
	{
		public float left;
		public float top;
		public float right;
		public float bottom;


		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(ref top, nameof(top), 0);
			serializer.Serialize(ref left, nameof(left), 0);
			serializer.Serialize(ref bottom, nameof(bottom), 0);
			serializer.Serialize(ref right, nameof(right), 0);
		}
	}
}