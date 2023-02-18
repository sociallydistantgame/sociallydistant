#nullable enable

using UnityEngine;

namespace UI.Windowing
{
	[CreateAssetMenu(menuName = "ScriptableObject/Services/Window Drag Service")]
	public class WindowDragService : ScriptableObject
	{
		public UguiWindow Window { get; private set; }

		public void StartDrag(UguiWindow window)
		{
			this.Window = window;
			
			
		}

		public void EndDrag()
		{
			this.Window = null;
		}
	}
}