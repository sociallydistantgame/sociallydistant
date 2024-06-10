using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TrixelCreative.TrixelAudio.Data
{
	[CreateAssetMenu(menuName = "TrixelAudio/Sound Bank")]
	public class SoundBankAsset : ScriptableObject
	{
		[SerializeField]
		private string soundBankName = string.Empty;

		[SerializeField]
		private SoundEffectAsset[] soundEffects = Array.Empty<SoundEffectAsset>();

		public bool TryGetRandomSound(out SoundEffectAsset sound)
		{
			sound = null!;
			if (this.soundEffects.Length == 0)
				return false;

			sound = this.soundEffects[Random.Range(0, this.soundEffects.Length)];
			return true;
		}
		
		public SoundEffectAsset? TryGetRandomSound()
		{
			// SoundEffectAsset sound = null!;
			if (this.soundEffects.Length == 0)
				return null;

			return this.soundEffects[Random.Range(0, this.soundEffects.Length)];
			//return sound;
		}
		
#if UNITY_EDITOR

		[Header("Import Sound Effects")]
		[SerializeField]
		private string assetFolder = string.Empty;

		public void Reimport()
		{
			bool shouldImport = EditorUtility.DisplayDialog(
				"Reimport sound effects?",
				"Re-importing from an assets folder will remove any manually-imported sound effects. You can add them back later if you'd like. Are you sure you want to do this?",
				"Yes",
				"No"
			);

			if (!shouldImport)
				return;

			var collectedAssets = new List<SoundEffectAsset>();

			string typename = $"t: {nameof(SoundEffectAsset)}";
			
			// Search in the root folder
			string[] rootAssets = AssetDatabase.FindAssets(typename, new[] { assetFolder });
			foreach (string guid in rootAssets)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);
				SoundEffectAsset asset = AssetDatabase.LoadAssetAtPath<SoundEffectAsset>(assetPath);
				collectedAssets.Add(asset);
			}
			
			// Search in subfolders
			string[] folders = AssetDatabase.GetSubFolders(this.assetFolder);
			foreach (string folder in folders)
			{
				string[] guids = AssetDatabase.FindAssets(typename, new[] { folder });

				foreach (string guid in guids)
				{
					string assetPath = AssetDatabase.GUIDToAssetPath(guid);
					SoundEffectAsset asset = AssetDatabase.LoadAssetAtPath<SoundEffectAsset>(assetPath);
					collectedAssets.Add(asset);
				}
			}

			this.soundEffects = collectedAssets.ToArray();

			EditorUtility.SetDirty(this);
		}

#endif
	}
}