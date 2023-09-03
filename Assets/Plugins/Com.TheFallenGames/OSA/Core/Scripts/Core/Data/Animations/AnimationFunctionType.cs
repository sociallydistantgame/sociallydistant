using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Com.TheFallenGames.OSA.Core.Data.Animations
{
	[Serializable]
	public enum AnimationFunctionType
	{
		SLOW_OUT,
		FAST_IN_SLOW_OUT,
		SLOW_IN_OUT,
		LINEAR
	}
}
