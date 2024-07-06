#nullable enable
using SociallyDistant.Core.Shell.Common;

namespace SociallyDistant.Core.Shell.Windowing
{
	public interface IWindowTab
	{
		bool Active { get; }
		string Title { get; }
	}
	
	public interface IWindow :  ICloseable
	{
		public event Action<IWindow>? WindowClosed; 

		WindowHints Hints { get; }
		IWorkspaceDefinition Workspace { get; }
		CompositeIcon Icon { get; set; }
		
		bool IsActive { get; }

		IWorkspaceDefinition CreateWindowOverlay();

		void SetWindowHints(WindowHints hints);
	}
}