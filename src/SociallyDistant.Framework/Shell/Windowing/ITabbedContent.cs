namespace SociallyDistant.Core.Shell.Windowing
{
	public interface ITabbedContent : IContentHolder
	{
		Action? NewTabCallback { get; set; }
		
		bool ShowNewTab { get; set; }
		
		IReadOnlyList<IContentPanel> Tabs { get; }
		
		void NextTab();
		void PreviousTab();
		void SwitchTab(int index);
		void CloseTab(int index);
		
		IContentPanel CreateTab();
		Task<bool> RemoveTab(IContentPanel panel);
	}
}