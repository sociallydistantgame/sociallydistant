#nullable enable
using Player;

namespace CustomInput
{
	public interface IInputManager
	{
		bool HandleInputs(PlayerInstance playerInstance, GameControls gameControls, bool consumedByOtherSystem);
	}
}