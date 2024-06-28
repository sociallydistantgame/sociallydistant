#nullable enable

namespace SociallyDistant.Core;

public static class Time
{
	private static TimeData? timeData;

	public static float deltaTime
	{
		get
		{
			// TODO: This is for Unity compat. We should switch to a double and rename the property.
			ThrowIfNotReady();
			return (float) timeData.FrameTime.TotalSeconds;
		}
	}
	
	public static float time
	{
		get
		{
			// TODO: This is for Unity compat. We should switch to a double and rename the property.
			ThrowIfNotReady();
			return (float) timeData.TotalTime.TotalSeconds;
		}
	}
	
	public static TimeData Initialize()
	{
		if (timeData != null)
			throw new InvalidOperationException("Initializing the Time class can only be done once and should only be done by SociallyDistantGame.");

		timeData = new();

		return timeData;
	}

	private static void ThrowIfNotReady()
	{
		if (timeData == null)
			throw new InvalidOperationException("The game has not been started.");
	}
}