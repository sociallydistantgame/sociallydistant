using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Com.TheFallenGames.OSA.Core.Data.Animations
{
	/// <summary>
	/// Parameters for cancelling certain animations on certain events
	/// </summary>
	[Serializable]
	public class AnimationCancelling
	{
		[SerializeField]
		[Tooltip("This decides whether or not to stop an existing SmoothScrollTo animation on certain events")]
		SmoothScrollCancellingEvents _SmoothScroll = new SmoothScrollCancellingEvents();
		/// <summary>
		/// This decides whether or not to stop an existing <see cref="OSA{TParams, TItemViewsHolder}.SmoothScrollTo(int, float, float, float, Func{float, bool}, Action, bool)"/> animation on certain events
		/// </summary>
		public SmoothScrollCancellingEvents SmoothScroll { get { return _SmoothScroll; } set { _SmoothScroll = value; } }

		[SerializeField]
		[Tooltip("Custom animations you may have. This decides whether or not to call CancelUserAnimations() on certain events, which you can override to comply")]
		UserAnimationsCancellingEvents _UserAnimations = new UserAnimationsCancellingEvents();
		/// <summary>
		/// Custom animations you may have. This decides whether or not to call <see cref="OSA{TParams, TItemViewsHolder}.CancelUserAnimations"/>, which you can override to comply
		/// </summary>
		public UserAnimationsCancellingEvents UserAnimations { get { return _UserAnimations; } set { _UserAnimations = value; } }
	}
}
