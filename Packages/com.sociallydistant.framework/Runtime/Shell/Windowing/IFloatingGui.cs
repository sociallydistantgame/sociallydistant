#nullable enable
using UnityEngine;

namespace Shell.Windowing
{
	public interface IFloatingGui : IWindow, IContentHolder
	{
		Vector2 MinimumSize { get; set; }
		Vector2 Position { get; set; }
		
		WindowState WindowState { get; set; }
		
		bool EnableCloseButton { get; set; }
	}
}