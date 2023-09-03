// Pre-2019.1 Unity versions have performance problems with the Shadow script,
// while 2019.{1,2,3,4} sometimes have problems with displaying an Image+Shadow
#if UNITY_2019_1_OR_NEWER && !(UNITY_2019_1_0 || UNITY_2019_1_1 || UNITY_2019_1_2 || UNITY_2019_1_3 || UNITY_2019_1_4)
#define ALLOW_SHADOWS
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.Util.Optimization
{
	/// <summary>
	/// Simple: If no Unity 2019, no shadow (the shadow will be destroyed). 
	/// Seems like a lot of Shadow components used cause significant FPS drops
	/// </summary>
	[ExecuteInEditMode]
	public class ShadowRemover : MonoBehaviour
	{
		// If not on a compatible unity version, destroy the Shadow script and this script
#if ALLOW_SHADOWS

#else
		[SerializeField]
		[HideInInspector]
		Shadow _Shadow;


		void Awake()
		{
			if (!_Shadow)
				_Shadow = GetComponent<Shadow>();

			if (!Application.isPlaying)
				return;

			// In play mode, destroy it
			if (_Shadow)
			{
				Destroy(_Shadow);
				_Shadow = null;
			}
			Destroy(this);
		}

#if UNITY_EDITOR
		void Update()
		{
			// No checks during play mode
			if (Application.isPlaying)
				return;

			if (!_Shadow)
				_Shadow = GetComponent<Shadow>();
		}
#endif

#endif
	}
}