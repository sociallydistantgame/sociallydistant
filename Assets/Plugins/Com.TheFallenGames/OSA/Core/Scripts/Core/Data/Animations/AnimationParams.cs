using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Com.TheFallenGames.OSA.Core.Data.Animations
{
	/// <summary>
	/// Parameters for animations in general
	/// </summary>
	[Serializable]
	public class AnimationParams
	{
		[SerializeField]
		AnimationFunctionType _SmoothScrollType = AnimationFunctionType.SLOW_OUT;
		public AnimationFunctionType SmoothScrollType { get { return _SmoothScrollType; } set { _SmoothScrollType = value; } }

		[SerializeField]
		AnimationCancelling _Cancel = new AnimationCancelling();
		public AnimationCancelling Cancel { get { return _Cancel; } set { _Cancel = value; } }

        public bool CallDoneOnScrollCancel { get { return _OnDoneWhenCancelled; } set { _OnDoneWhenCancelled = value; } }

		[SerializeField]
		bool _OnDoneWhenCancelled;
    }
}
