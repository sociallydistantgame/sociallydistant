using System;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Music
{
	[RequireComponent(typeof(Collider))]
	public class JukeboxTrigger : JukeboxBase
	{
		private void OnTriggerEnter(Collider other)
		{
			Play();
		}
	}
}