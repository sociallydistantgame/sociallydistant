#nullable enable
using UI.Backdrop;
using UI.Windowing;
using UnityEngine;

namespace Player
{
	public struct PlayerInstance
	{
		public GameObject UiRoot;
		public BackdropController BackdropController;
		public WindowManager WindowManager;
	}
}