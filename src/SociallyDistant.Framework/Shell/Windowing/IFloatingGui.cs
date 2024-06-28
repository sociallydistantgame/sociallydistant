#nullable enable
using Microsoft.Xna.Framework;

namespace SociallyDistant.Core.Shell.Windowing
{
	public interface IFloatingGui : IWindow, IContentHolder
	{
		Vector2 MinimumSize { get; set; }
		Vector2 Position { get; set; }
		
		WindowState WindowState { get; set; }
		
		bool EnableCloseButton { get; set; }
	}
}