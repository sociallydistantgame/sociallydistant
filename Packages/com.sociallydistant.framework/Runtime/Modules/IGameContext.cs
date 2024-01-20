#nullable enable
using ContentManagement;
using Core;
using Core.Config;
using Core.Scripting;
using OS;
using Shell;
using Shell.InfoPanel;
using Social;

namespace Modules
{
	/// <summary>
	///		Represents the core of Socially Distant.
	/// </summary>
	public interface IGameContext
	{
		TabbedToolCollection AvailableTools { get; }
		
		GameMode CurrentGameMode { get; }
		
		ISocialService SocialService { get; }
		
		/// <summary>
		///		Gets a reference to the player kernel.
		/// </summary>
		IKernel Kernel { get; }
		
		/// <summary>
		///		Gets a reference to the game's UI manager.
		/// </summary>
		IShellContext Shell { get; }
		
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
		
		/// <summary>
		///		Gets a reference to the script system.
		/// </summary>
		IScriptSystem ScriptSystem { get; }
	}
}