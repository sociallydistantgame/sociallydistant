using AcidicGui.Widgets;

namespace UI.Widgets.Settings
{
	public abstract class SettingsWidgetController<T> : WidgetController
		where T : SettingsWidget
	{
		private T widget;

		protected T Widget => widget;

		public void Setup(T widget)
		{
			this.widget = widget;
		}
	}
}