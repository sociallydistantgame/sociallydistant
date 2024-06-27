#nullable enable

using UI.Tweening.Base;
using UnityEngine;

namespace UI.Tweening.ScaleTween
{
	[CreateAssetMenu(menuName = "ScriptableObject/Tweening/Scale Tweener Settings")]
	public class ScaleTweenerSettings : TweenerSettings<Vector3>
	{
		/// <inheritdoc />
		public override Vector3 GetValue(float percentage)
		{
			return Vector3.Lerp(STartValue, EndValue, percentage);
		}
	}
}