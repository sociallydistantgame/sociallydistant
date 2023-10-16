#nullable enable
using System;
using UI.Themes.Serialization;
using UnityEngine;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class BackdropStyle : IThemeData
	{
		[SerializeField]
		private ThemeGraphic dayTimeBackdrop = new ThemeGraphic();
		
		[SerializeField]
		private ThemeGraphic nightTimeBackdrop = new ThemeGraphic();
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(dayTimeBackdrop, assets, nameof(dayTimeBackdrop));
			serializer.Serialize(nightTimeBackdrop, assets, nameof(nightTimeBackdrop));
		}
	}
}