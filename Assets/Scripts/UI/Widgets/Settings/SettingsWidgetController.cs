using AcidicGui.Widgets;

namespace UI.Widgets.Settings
{
	public abstract class SettingsWidgetController<T> : WidgetController
		where T : SettingsWidget
	{
		public abstract void Setup(T widget);
	}
}