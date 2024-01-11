namespace Shell.Windowing
{
	public interface ICloseable
	{
		bool CanClose { get; }
		
		void Close();
		void ForceClose();
	}
}