namespace Shell.Windowing
{
	public interface IWindowWithClient<TWindowClient> : IWindow
	{
		TWindowClient Client { get; }
		
		void SetClient(TWindowClient newClient);
	}
}