using System;

namespace Com.TheFallenGames.OSA.Demos.Common
{
	[Obsolete("Use Util.Animations.ExpandCollapseAnimationState instead")]
	public class ExpandCollapseAnimationState : Util.Animations.ExpandCollapseAnimationState
	{
		public ExpandCollapseAnimationState(bool useUnscaledTime) : base(useUnscaledTime)
		{
		}
	}
}