namespace SociallyDistant.Core.Shell.Windowing
{
	public interface IWindowWithClient<TWindowClient> : IWindow
	{
		string Title { get; set; }
		TWindowClient Client { get; }
		
		void SetClient(TWindowClient newClient);
	}
}