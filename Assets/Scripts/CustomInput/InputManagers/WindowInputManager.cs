#nullable enable

using Player;
using UI.Windowing;

namespace CustomInput.InputManagers
{
	public class WindowInputManager : IInputManager
	{
		private WindowFocusService focusService;
		
		public WindowInputManager(WindowFocusService focusService)
		{
			this.focusService = focusService;
		}
		
		/// <inheritdoc />
		public bool HandleInputs(PlayerInstance playerInstance, GameControls gameControls, bool consumedByOtherSystem)
		{
			if (playerInstance.UiManager.WindowManager == null)
				return false;

			return false;
		}
	}
}