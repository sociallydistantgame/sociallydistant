namespace SociallyDistant.Core.Shell.Windowing
{
	public interface ICloseable
	{
		bool CanClose { get; set; }
		
		void Close();
		void ForceClose();
	}
}