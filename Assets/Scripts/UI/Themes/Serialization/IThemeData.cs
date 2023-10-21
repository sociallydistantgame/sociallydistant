#nullable enable
using AcidicGui.Widgets;
using System;
using UI.Theming;

namespace UI.Themes.Serialization
{
	public interface IThemeData
	{
		void Serialize(IElementSerializer serializer, ThemeAssets assets);
		void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext);
	}
}