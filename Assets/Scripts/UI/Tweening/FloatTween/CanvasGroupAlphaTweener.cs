#nullable enable
using UnityEngine;
using UnityExtensions;
using Utility;

namespace UI.Tweening.FloatTween
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasGroupAlphaTweener : FloatTweener
	{
		private CanvasGroup canvasGroup = null!;
		
		protected override void OnAwake()
		{
			this.MustGetComponent(out canvasGroup);
		}

		/// <inheritdoc />
		protected override void Apply(float newValue)
		{
			canvasGroup.alpha = newValue;
		}
	}
}