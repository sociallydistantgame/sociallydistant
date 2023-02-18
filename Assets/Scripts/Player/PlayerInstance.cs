#nullable enable
using UI.Backdrop;
using UI.Shell;
using UI.Windowing;
using UnityEngine;

namespace Player
{
	public struct PlayerInstance
	{
		public GameObject UiRoot;
		public BackdropController BackdropController;
		public Desktop Desktop;
		public WindowManager WindowManager;
	}
}