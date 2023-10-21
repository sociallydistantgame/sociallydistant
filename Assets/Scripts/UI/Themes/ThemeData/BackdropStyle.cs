#nullable enable
using System;
using AcidicGui.Widgets;
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

		public ThemeGraphic DayTime => dayTimeBackdrop;
		public ThemeGraphic NightTime => nightTimeBackdrop;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(dayTimeBackdrop, assets, nameof(dayTimeBackdrop));
			serializer.Serialize(nightTimeBackdrop, assets, nameof(nightTimeBackdrop));
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			builder.PushDefaultSection("Day-time Backdrop", out _);
			dayTimeBackdrop.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
			
			builder.PushDefaultSection("Night-time Backdrop", out _);
			nightTimeBackdrop.BuildWidgets(builder, markDirtyAction, editContext);
			builder.PopDefaultSection();
		}
	}
}