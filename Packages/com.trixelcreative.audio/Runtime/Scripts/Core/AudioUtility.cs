#nullable enable

using UnityEngine;

namespace TrixelCreative.TrixelAudio.Core
{
	public static class AudioUtility
	{
		private const float DecibelScale = 20f;
		
		public static float PercentageToDb(float percentage)
		{
			// Clamp the value such that it is never zero.
			percentage = Mathf.Clamp(percentage, 0.001f, 1f);
			return Mathf.Log10(percentage) * DecibelScale;
		}

		public static float DbToPercentage(float decibels)
		{
			decibels = Mathf.Clamp(decibels, -80, 0);
			return Mathf.Pow(decibels / DecibelScale, 10);
		}
	}
}