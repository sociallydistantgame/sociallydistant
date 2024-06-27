using System;
using TrixelCreative.TrixelAudio.Utility;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Players
{
	public abstract class AudioPlayerBase : MonoBehaviour
	{
		private void Awake()
		{
			OnAwake();
		}
		
		protected virtual void OnAwake() { }
	}
}