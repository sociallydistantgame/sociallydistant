#nullable enable

using System;
using UnityEngine;

namespace UI.Tweening.Base
{
	public abstract class TweenerSettings<T> : ScriptableObject
		where T : struct
	{
		[Header("Settings")]
		
		private float durationInSeconds;

		
		private CurveFunction curveFunction;

		
		private EasingMode easingMode;
		
		[Header("Values")]
		
		private T startValue;

		
		private T endValue;
		
		public float DurationSeconds => durationInSeconds;
		public T STartValue => startValue;
		public T EndValue => endValue;
		public CurveFunction CurveFunction => curveFunction;
		public EasingMode EasingMode => easingMode;
		
		public abstract T GetValue(float percentage);
	}
}