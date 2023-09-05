#nullable enable

namespace Shell.InfoPanel
{
	public interface IInfoPanelService
	{
		int CreateCloseableInfoWidget(string icon, string title, string message);

		int CreateStickyInfoWidget(string icon, string title, string message);
	}
}