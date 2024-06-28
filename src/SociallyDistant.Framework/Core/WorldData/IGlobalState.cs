#nullable enable

namespace SociallyDistant.Core.Core.WorldData
{
	public interface IGlobalState
	{
		DateTime Now { get; }
		float TimeScale { get; }
	}
}