#if UNITY_EDITOR
#nullable enable
using System;
using TrixelCreative.TrixelAudio.Data;
using UnityEditor;
using UnityEngine;

namespace TrixelCreative.TrixelAudio.Editor.CustomInspectors
{
	[CustomEditor(typeof(SoundBankAsset))]
	public class SoundBankEditor : UnityEditor.Editor
	{
		private SoundBankAsset currentAsset = null!;

		private void OnEnable()
		{
			this.currentAsset = (SoundBankAsset) target;
		}

		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Reimport"))
			{
				currentAsset.Reimport();
			}
		}
	}
}
#endif