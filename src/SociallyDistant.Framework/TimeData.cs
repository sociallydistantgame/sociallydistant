using Microsoft.Xna.Framework;

namespace SociallyDistant.Core;

public sealed class TimeData
{
	private GameTime lastUpdate = new();

	public TimeSpan TotalTime => lastUpdate.TotalGameTime;
	public TimeSpan FrameTime => lastUpdate.ElapsedGameTime;
	
	public void Update(GameTime gameTime)
	{
		lastUpdate = gameTime;
	}
}