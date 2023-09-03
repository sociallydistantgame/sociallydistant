using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Com.TheFallenGames.OSA.Core.Data.Animations
{
	[Serializable]
	public class UserAnimationsCancellingEvents : AnimationCancellingEvents
	{
		[SerializeField]
		[Tooltip("Whether to cancel on OSA.SmoothScrollTo")]
		bool _OnBeginSmoothScroll = true;
		/// <summary>Whether to cancel on <see cref="OSA{TParams, TItemViewsHolder}.SmoothScrollTo(int, float, float, float, Func{float, bool}, Action, bool)</summary>
		public bool OnBeginSmoothScroll { get { return _OnBeginSmoothScroll; } set { _OnBeginSmoothScroll = value; } }
	}
}
