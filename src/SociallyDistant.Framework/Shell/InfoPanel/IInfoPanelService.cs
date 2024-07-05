#nullable enable

namespace SociallyDistant.Core.Shell.InfoPanel
{
	public interface IInfoPanelService
	{
		int CreateCloseableInfoWidget(string icon, string title, string message);

		int CreateStickyInfoWidget(string icon, string title, string message);

		void CloseWidget(int widgetId);
	}
}