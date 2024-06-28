#nullable enable
namespace SociallyDistant.Core.Shell.Windowing
{
	/// <summary>
	///		Data used to control the behaviour and appearance of a <see cref="IWindow"/>.
	/// </summary>
	[Serializable]
	public struct WindowHints
	{
		/// <summary>
		///		Determines whether the window's background is provided by the client UI.
		///		If set to true, the window decoration will not provide a background color for the window.
		///		This is useful for creating effects in a window such as the translucent, blurred background
		///		of Terminal.
		/// </summary>
		public bool ClientRendersWindowBackground;
	}
}