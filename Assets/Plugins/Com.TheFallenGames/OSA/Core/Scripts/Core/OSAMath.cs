using Com.TheFallenGames.OSA.Core.Data.Animations;
using System;

namespace Com.TheFallenGames.OSA.Core
{
	public class OSAMath
	{
		public static double Lerp_SinSlowOut(double t01)
		{
			// Normal in, sin slow out
			return Math.Sin(t01 * Math.PI / 2);
		}

		public static double Lerp_SqrtFastInSlowOut(double t01)
		{
			// fast-in, slow-out effect
			return Math.Sqrt(t01);
		}

		public static double Lerp_ExpSlowInOut(double t01)
		{
			return t01 * t01 * (3f - 2f * t01);
		}

		public static double Lerp_Linear(double t01)
		{
			return t01;
		}

		public static Func<double, double> GetLerpFunction(AnimationFunctionType animationFunctionType)
		{
			switch (animationFunctionType)
			{
				case AnimationFunctionType.LINEAR:
					return Lerp_Linear;

				case AnimationFunctionType.SLOW_OUT:
					return Lerp_SinSlowOut;

				case AnimationFunctionType.FAST_IN_SLOW_OUT:
					return Lerp_SqrtFastInSlowOut;

				case AnimationFunctionType.SLOW_IN_OUT:
					return Lerp_ExpSlowInOut;

				default:
					throw new OSAException("AnimationFunctionType not handled: " + animationFunctionType);
			}
		}
	}
}
