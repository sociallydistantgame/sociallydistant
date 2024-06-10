#nullable enable

using UnityEngine;
using UnityEngine.Assertions;

namespace TrixelCreative.TrixelAudio.Utility
{
	internal static class UnityHelpers
	{
		public static void MustGetComponent<T>(this MonoBehaviour script, out T component) where T : UnityEngine.Object
		{
			component = script.GetComponent<T>();
			Assert.IsNotNull(component, $"Missing required component of type {typeof(T).FullName} needed on object \"{script.name}\"");
		}
	}
}