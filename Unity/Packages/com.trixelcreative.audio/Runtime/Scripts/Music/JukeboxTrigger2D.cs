using System;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Music
{
	[RequireComponent(typeof(Collider2D))]
	public class JukeboxTrigger2D : JukeboxBase
	{
		private void OnTriggerEnter2D(Collider2D col)
		{
			Play();
		}
	}
}