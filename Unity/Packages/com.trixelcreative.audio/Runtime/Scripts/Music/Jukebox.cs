using System;
using TrixelCreative.TrixelAudio.Data;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Music
{
	public class Jukebox : JukeboxBase
	{
		
		private bool playOnAwake = true;
		
		private void Start()
		{
			if (playOnAwake)
				Play();
		}
	}
}