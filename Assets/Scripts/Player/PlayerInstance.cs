#nullable enable
using OS.Devices;
using UI.Backdrop;
using UI.Login;
using UI.Popovers;
using UI.Shell;
using UI.Windowing;
using UnityEngine;

namespace Player
{
	public struct PlayerInstance
	{
		public IInitProcess OsInitProcess;
		public PlayerComputer Computer;
		
		public GameObject UiRoot;
		public BackdropController BackdropController;
		public Desktop Desktop;
		public WindowManager WindowManager;
		public LoginManager LoginManager;
		public PopoverLayer PopoverLayer;
	}
}