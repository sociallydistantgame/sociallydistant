#nullable enable

using UI.Tweening.Base;
using UnityEngine;

namespace UI.Tweening.FloatTween
{
	[CreateAssetMenu(menuName = "ScriptableObject/Tweening/Float Tweener Settings")]
	public class FloatTweenSettings : TweenerSettings<float>
	{
		/// <inheritdoc />
		public override float GetValue(float percentage)
		{
			return Mathf.Lerp(STartValue, EndValue, percentage);
		}
	}
}