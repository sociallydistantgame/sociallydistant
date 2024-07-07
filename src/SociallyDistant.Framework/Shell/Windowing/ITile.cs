#nullable enable
namespace SociallyDistant.Core.Shell.Windowing
{
	public interface ITile : 
		IWindow, 
		ITabbedContent
	{
		bool ShowNewTab { get; set; }
		
		void Hide();
		void Show();
	}
}