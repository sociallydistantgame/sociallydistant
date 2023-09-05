#nullable enable

using System;

namespace Core.WorldData
{
	public interface IGlobalState
	{
		DateTime Now { get; }
		float TimeScale { get; }
	}
}