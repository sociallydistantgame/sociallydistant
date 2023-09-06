#nullable enable
using ContentManagement;
using Core;
using Core.Config;
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
		
		/// <summary>
		///		Gets a reference to the world manager, which manages the state of the game's world.
		/// </summary>
		IWorldManager WorldManager { get; }
		
		/// <summary>
		///		Gets a reference to the game's settings manager.
		/// </summary>
		ISettingsManager SettingsManager { get; }
	}
}