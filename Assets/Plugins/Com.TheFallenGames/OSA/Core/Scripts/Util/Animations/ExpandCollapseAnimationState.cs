using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core.Data.Animations;
using System;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.Util.Animations
{
	/// <summary>
	/// Used for more control than what <see cref="Com.TheFallenGames.OSA.Util.ExpandCollapseOnClick"/> offers.
	/// Holds all the required data for animating an item's size. The animation is done manually, using a MonoBehaviour's Update
	/// </summary>
	public class ExpandCollapseAnimationState
	{
		public readonly float expandingStartTime;
		public float initialExpandedAmount;
		public float targetExpandedAmount;
		public float duration;
		public int itemIndex;
		bool _ForcefullyFinished;
		readonly bool _UseUnscaledTime;
		readonly Func<double, double> _Fn;

		/// <summary>
		/// Returns a value between <see cref="initialExpandedAmount"/> and <see cref="targetExpandedAmount"/> lerped by <see cref="Progress01"/>
		/// </summary>
		public float CurrentExpandedAmount { get { return Mathf.Lerp(initialExpandedAmount, targetExpandedAmount, Progress01); } }

		/// <summary>
		/// Returns a value between <see cref="targetExpandedAmount"/> and <see cref="initialExpandedAmount"/> lerped by <see cref="Progress01"/>
		/// </summary>
		public float CurrentExpandedAmountInverse { get { return Mathf.Lerp(targetExpandedAmount, initialExpandedAmount, Progress01); } }

		public float Progress01 { get { return CurrentAnimationElapsedTimeSmooth01; } }

		public bool IsDone { get { return CurrentAnimationElapsedTime01 == 1f; } }

		public bool IsExpanding { get { return targetExpandedAmount > initialExpandedAmount; } }

		float CurrentAnimationElapsedTime01
		{
			get
			{
				if (_ForcefullyFinished)
					return 1f;

				// Prevent div by zero. Also, no duration means there's no animation over time
				if (duration == 0f)
					return 1f;

				var elapsed01 = (Time - expandingStartTime) / duration;

				if (elapsed01 >= 1f)
					elapsed01 = 1f;

				return elapsed01;
			}
		}

		float CurrentAnimationElapsedTimeSmooth01
		{
			get
			{
				var t = CurrentAnimationElapsedTime01;
				if (t == 1f)
					return t;

				return (float)_Fn(t);
			}
		}

		float Time { get { return _UseUnscaledTime ? UnityEngine.Time.unscaledTime : UnityEngine.Time.time; } }


		public ExpandCollapseAnimationState(bool useUnscaledTime, AnimationFunctionType animationFunctionType = AnimationFunctionType.SLOW_OUT)
		{
			_UseUnscaledTime = useUnscaledTime;
			_Fn = OSAMath.GetLerpFunction(animationFunctionType);
			expandingStartTime = Time;
		}


		public void ForceFinish()
		{
			_ForcefullyFinished = true;
		}
	}
}