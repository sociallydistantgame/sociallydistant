#nullable enable
using System;
using System.Collections;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace UI.Tweening.Base
{
	public abstract class Tweener<TTweenerSettings, TValueType> : MonoBehaviour
		where TTweenerSettings : TweenerSettings<TValueType>
		where TValueType : struct
	{
		private Coroutine? tweenCoroutine;
		
		[Header("Settings")]
		[SerializeField]
		private TTweenerSettings tweenerSettings = null!;

		[SerializeField]
		private float timeScale = 1;

		[SerializeField]
		private PlayMode defaultPlayMode;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(Tweener<TTweenerSettings, TValueType>));
			
			OnAwake();
		}

		protected virtual void OnAwake() {}

		protected abstract void Apply(TValueType newValue);

		public void Stop()
		{
			if (tweenCoroutine == null)
				return;
			
			StopCoroutine(tweenCoroutine);
			tweenCoroutine = null;
		}

		public void Play()
		{
			Play(defaultPlayMode);
		}
		
		public void Play(PlayMode playMode)
		{
			Stop();

			tweenCoroutine = StartCoroutine(Tween(playMode));
		}

		private Func<float, float> GetCurveFunction()
		{
			EasingMode easingMode = tweenerSettings.EasingMode;
			
			switch (tweenerSettings.CurveFunction)
			{
				case CurveFunction.Quad:
					return easingMode switch
					{
						EasingMode.In => CurveFunctions.InQuad,
						EasingMode.Out => CurveFunctions.OutQuad,
						EasingMode.InOut => CurveFunctions.InOutQuad,
						_ => CurveFunctions.Linear
					};
				case CurveFunction.Cubic:
					return easingMode switch
					{
						EasingMode.In => CurveFunctions.InCubic,
						EasingMode.Out => CurveFunctions.OutCubic,
						EasingMode.InOut => CurveFunctions.InOutCubic,
						_ => CurveFunctions.Linear
					};
				case CurveFunction.Quart:
					return easingMode switch
					{
						EasingMode.In => CurveFunctions.InQuart,
						EasingMode.Out => CurveFunctions.OutQuart,
						EasingMode.InOut => CurveFunctions.InOutQuart,
						_ => CurveFunctions.Linear
					};
				case CurveFunction.Quint:
					return easingMode switch
					{
						EasingMode.In => CurveFunctions.InQuint,
						EasingMode.Out => CurveFunctions.OutQuint,
						EasingMode.InOut => CurveFunctions.InOutQuint,
						_ => CurveFunctions.Linear
					};
				case CurveFunction.Sine:
					return easingMode switch
					{
						EasingMode.In => CurveFunctions.InSine,
						EasingMode.Out => CurveFunctions.OutSine,
						EasingMode.InOut => CurveFunctions.InOutSine,
						_ => CurveFunctions.Linear
					};
				case CurveFunction.Expo:
					return easingMode switch
					{
						EasingMode.In => CurveFunctions.InExpo,
						EasingMode.Out => CurveFunctions.OutExpo,
						EasingMode.InOut => CurveFunctions.InOutExpo,
						_ => CurveFunctions.Linear
					};
				case CurveFunction.Circ:
					return easingMode switch
					{
						EasingMode.In => CurveFunctions.InCirc,
						EasingMode.Out => CurveFunctions.OutCirc,
						EasingMode.InOut => CurveFunctions.InOutCirc,
						_ => CurveFunctions.Linear
					};
				case CurveFunction.Elastic:
					return easingMode switch
					{
						EasingMode.In => CurveFunctions.InElastic,
						EasingMode.Out => CurveFunctions.OutElastic,
						EasingMode.InOut => CurveFunctions.InOutElastic,
						_ => CurveFunctions.Linear
					};
				case CurveFunction.Back:
					return easingMode switch
					{
						EasingMode.In => CurveFunctions.InBack,
						EasingMode.Out => CurveFunctions.OutBack,
						EasingMode.InOut => CurveFunctions.InOutBack,
						_ => CurveFunctions.Linear
					};
				case CurveFunction.Bounce:
					return easingMode switch
					{
						EasingMode.In => CurveFunctions.InBounce,
						EasingMode.Out => CurveFunctions.OutBounce,
						EasingMode.InOut => CurveFunctions.InOutBounce,
						_ => CurveFunctions.Linear
					};
			}
			
			return CurveFunctions.Linear;
		}

		private IEnumerator Tween(PlayMode playMode)
		{
			Func<float, float> curveFunction = GetCurveFunction();
			
			float time = 0;
			float duration = this.tweenerSettings.DurationSeconds;

			if (playMode != PlayMode.Backward)
			{
				while (time < duration)
				{
					float t = time / duration;
					float curved = curveFunction(t);

					Apply(tweenerSettings.GetValue(curved));

					time += Time.deltaTime * timeScale;
					yield return null;
				}
			}

			if (playMode != PlayMode.Forward)
			{
				time = 0;
				while (time < duration)
				{
					float t = 1 - (time / duration);
					float curved = curveFunction(t);

					Apply(tweenerSettings.GetValue(curved));

					time += Time.deltaTime * timeScale;
					yield return null;
				}
			}

			Stop();
		}
	}
}