#nullable enable
using UnityEngine;

namespace Shell.Windowing
{
	public interface IFloatingGui : IWindow
	{
		Vector2 MinimumSize { get; set; }
		Vector2 Position { get; set; }
		
		WindowState WindowState { get; set; }
		
		bool EnableCloseButton { get; set; }
		bool EnableMaximizeButton { get; set; }
		bool EnableMinimizeButton { get; set; }
		
		void ToggleMaximize();
		void Restore();
		void Minimize();
	}
}