#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modules
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

		protected IGameContext Context { get; private set; } = null!;
		
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
		
	}
}