#nullable enable

using UnityEngine;

namespace UI.Windowing
{
	[CreateAssetMenu(menuName = "ScriptableObject/Services/Window Focus Service")]
	public class WindowFocusService : ScriptableObject
	{
		private IWindow? focusedWindow = null!;

		public IWindow? FocusedWindow => focusedWindow;
		
		public void SetWindow(IWindow? window)
		{
			focusedWindow = window;
		}
	}
}