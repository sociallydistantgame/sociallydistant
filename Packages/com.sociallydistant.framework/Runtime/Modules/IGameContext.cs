#nullable enable
using ContentManagement;
using Shell;
using Shell.InfoPanel;

namespace Modules
{
	/// <summary>
	///		Represents the core of Socially Distant.
	/// </summary>
	public interface IGameContext
	{
		/// <summary>
		///		Gets an instance of the ContentManager.
		/// </summary>
		IContentManager ContentManager { get; }
		
		/// <summary>
		///		Gets a reference to the Info Panel Service, which manages the state of the desktop's information panel widgets.
		/// </summary>
		IInfoPanelService InfoPanelService { get; }
	}
}