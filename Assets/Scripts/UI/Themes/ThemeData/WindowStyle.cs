using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class WindowStyle : IThemeData
	{
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
		}
	}
}