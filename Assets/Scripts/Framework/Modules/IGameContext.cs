#nullable enable
using ContentManagement;
using Core;
using Core.Config;
using Core.Scripting;
using OS;
using Shell;
using Shell.InfoPanel;
using Social;
using System.Threading.Tasks;

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
		///		Gets the path on the filesystem to where the current save's data is stored. Will return null if no save data is loaded
		///		or if the current save isn't local and persistent (i.e, if we are a client on a network game or if the current game is in-memory.)
		/// </summary>
		string? CurrentSaveDataDirectory { get; }
		
		IUriManager UriManager { get; }
		
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

		Task SaveCurrentGame(bool silent);
		Task EndCurrentGame(bool save);
		
		bool IsDebugWorld { get; }

		void SetPlayerHostname(string hostname);
	}
}