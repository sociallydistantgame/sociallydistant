namespace Shell.Windowing
{
	public interface ITAbRenderer
	{
		ITabbedContent? TabbedContent { get; set; }
		
		void UpdateTabs();
	}
}