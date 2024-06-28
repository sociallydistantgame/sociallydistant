#nullable enable

using SociallyDistant.Core.Core;

namespace SociallyDistant.Core.Modules
{
	/// <summary>
	///		Class representing the entry point of a Socially Distant module.
	/// </summary>
	public abstract class GameModule
	{
		private bool isInitialized = false;

		/// <summary>
		///		Gets a value indicating whether this module is currently initialized.
		/// </summary>
		public bool IsInitialized => isInitialized;
		
		/// <summary>
		///		Gets a value indicating whether this module is a core module. If true, then the user can't enable or disable this module during runtime.
		/// </summary>
		public abstract bool IsCoreModule { get; }
		
		/// <summary>
		///		Gets a unique module ID for this module. Please use reverse domain name notation. 
		/// </summary>
		public abstract string ModuleId { get; }

		/// <summary>
		///		Gets a list of module IDs this module requires.
		/// </summary>
		public virtual IEnumerable<string> RequiredModules => Enumerable.Empty<string>();

		protected abstract Task OnInitialize();
		protected abstract Task OnShutdown();

		public IGameContext Context { get; private set; } = null!;
		
		/// <summary>
		///		Initializes the module if it is not already initialized.
		/// </summary>
		public async Task Initialize(IGameContext context)
		{
			if (isInitialized)
				return;

			this.Context = context;
			await OnInitialize();
			isInitialized = true;
			
			OnGameModeChanged(context.CurrentGameMode);
		}
		
		/// <summary>
		///		Shuts the module down.
		/// </summary>
		public async Task Shutdown()
		{
			if (!isInitialized)
				return;
            
			await OnShutdown();
			this.Context = null!;
			isInitialized = false;
		}

		/// <summary>
		///		Called when the current game mode changes. Use this to perform actions during certain game states.
		/// </summary>
		/// <param name="gameMode">The new game mode</param>
		public virtual void OnGameModeChanged(GameMode gameMode) { }

		/// <summary>
		///		Called every time the game's Content Manager reloads.
		/// </summary>
		public virtual void AfterContentLoad() {}
	}
}