namespace SociallyDistant.Core.Shell.Windowing
{
	public interface ITAbRenderer
	{
		ITabbedContent? TabbedContent { get; set; }
		
		void ScheduleUpdateTabs();
	}
}