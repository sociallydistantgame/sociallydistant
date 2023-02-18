#nullable enable

using Player;
using UI.Windowing;

namespace CustomInput.InputManagers
{
	public class WindowInputManager : IInputManager
	{
		private WindowDragService dragService;
		
		public WindowInputManager(WindowDragService dragService)
		{
			this.dragService = dragService;
		}
		
		/// <inheritdoc />
		public bool HandleInputs(PlayerInstance playerInstance, GameControls gameControls, bool consumedByOtherSystem)
		{
			if (playerInstance.WindowManager == null)
				return false;

			return false;
		}
	}
}