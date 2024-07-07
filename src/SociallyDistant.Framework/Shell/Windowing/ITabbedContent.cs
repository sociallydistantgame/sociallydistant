namespace SociallyDistant.Core.Shell.Windowing
{
	public interface ITabbedContent : IContentHolder
	{
		Action? NewTabCallback { get; set; }
		
		bool ShowNewTab { get; set; }
		
		IReadOnlyList<IContentPanel> ContentPanels { get; }
		
		void NextTab();
		void PreviousTab();
		void SwitchTab(int index);
		void CloseTab(int index);
		
		IContentPanel CreateTab(); 
		bool RemoveTab(IContentPanel panel);
	}
}