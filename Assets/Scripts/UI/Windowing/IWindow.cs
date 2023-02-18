using UnityEngine;

namespace UI.Windowing
{
	public interface IWindow<TWindowClient>
	{
		string Title { get; set; }
		WindowState WindowState { get; set; }
		
		bool EnableCloseButton { get; set; }
		bool EnableMaximizeButton { get; }
		bool EnableMinimizeButton { get; }
		
		bool IsActive { get; }
		
		Vector2 MinimumSize { get; set; }
		
		TWindowClient Client { get; }

		void SetClient(TWindowClient newClient);
	}
}