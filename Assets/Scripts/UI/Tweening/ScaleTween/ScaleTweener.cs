#nullable enable
using UI.Tweening.Base;
using UnityEngine;

namespace UI.Tweening.ScaleTween
{
	public class ScaleTweener : Tweener<ScaleTweenerSettings, Vector3>
	{
		/// <inheritdoc />
		protected override void Apply(Vector3 newValue)
		{
			transform.localScale = newValue;
		}
	}
}