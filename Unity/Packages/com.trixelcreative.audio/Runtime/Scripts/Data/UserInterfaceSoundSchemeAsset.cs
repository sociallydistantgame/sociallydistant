using TrixelCreative.TrixelAudio.Core;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Data
{
	[CreateAssetMenu(menuName = "TrixelAudio/User Interface Sound Scheme")]
	public class UserInterfaceSoundSchemeAsset : ScriptableObject
	{
		
		private SoundEffectAsset selectSound = null!;

		
		private SoundEffectAsset navigateSound = null!;

		
		private SoundEffectAsset cancelSound = null!;

		public void PlaySelectSound()
		{
			AudioManager.PlaySound(selectSound);
		}
		
		public void PlayNavigateSound()
		{
			AudioManager.PlaySound(navigateSound);
		}
		
		public void PlayCancelSound()
		{
			AudioManager.PlaySound(cancelSound);
		}
		
		


	}
}