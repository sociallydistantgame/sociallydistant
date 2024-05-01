#nullable enable

using System;
using UnityEngine;

namespace UI.Tweening.Base
{
	public abstract class TweenerSettings<T> : ScriptableObject
		where T : struct
	{
		[Header("Settings")]
		[SerializeField]
		private float durationInSeconds;

		[SerializeField]
		private CurveFunction curveFunction;

		[SerializeField]
		private EasingMode easingMode;
		
		[Header("Values")]
		[SerializeField]
		private T startValue;

		[SerializeField]
		private T endValue;
		
		public float DurationSeconds => durationInSeconds;
		public T STartValue => startValue;
		public T EndValue => endValue;
		public CurveFunction CurveFunction => curveFunction;
		public EasingMode EasingMode => easingMode;
		
		public abstract T GetValue(float percentage);
	}
}