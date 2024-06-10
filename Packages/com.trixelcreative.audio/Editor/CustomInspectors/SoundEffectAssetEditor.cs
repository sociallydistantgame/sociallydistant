#if UNITY_EDITOR
#nullable enable

using System;
using System.Collections;
using TrixelCreative.TrixelAudio.Data;
using UnityEditor;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Editor.CustomInspectors
{
	[CustomEditor(typeof(SoundEffectAsset))]
	public class SoundEffectAssetEditor : UnityEditor.Editor
	{
		private SoundEffectAsset currentAsset = null!;

		private void OnEnable()
		{
			this.currentAsset = (SoundEffectAsset) target;
		}

		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Preview Sound Effect"))
			{
				PreviewSound();
			}
		}

		private void PreviewSound()
		{
			GameObject go = new GameObject("TrixelAudio Preview Sound Player");
			AudioSource audioSource = go.AddComponent<AudioSource>();
			
			this.currentAsset.PlayOnAudioSource(audioSource);

			Destroyer destroyer = go.AddComponent<Destroyer>();
			destroyer.StartCoroutine(destroyer.DestroyAfterPlay(audioSource));
		}

		private class Destroyer : MonoBehaviour
		{
			public IEnumerator DestroyAfterPlay(AudioSource source)
			{
				while (source.isPlaying)
					yield return null;

				DestroyImmediate(this);
			}
		}
	}
}
#endif