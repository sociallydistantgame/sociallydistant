using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace TrixelCreative.TrixelAudio
{
	public class AudioSourcePool
	{
		private readonly AudioSource[] pool = Array.Empty<AudioSource>();
		private readonly GameObject poolRoot;
		private int highestAcquiredIndex = -1;
		private readonly string poolName;

		public AudioSourcePool(int poolSize, string poolName)
		{
			poolRoot = new GameObject(poolName);
			this.poolName = poolName;
			
			Assert.IsFalse(poolSize < 1, "[TrixelAudio] Sound effect pool size is below 1.");
			
			// Pre-allocate the pool
			this.pool = new AudioSource[poolSize];

			this.Initialize();
		}

		private void Initialize()
		{
			var poolManager = poolRoot.AddComponent<AudioPoolHelper>();
			poolManager.Pool = this;
			
			highestAcquiredIndex = -1;
			
			for (var i = 0; i < pool.Length; i++)
			{
				// Consume any existing AudioSources if they're not destroyed.
				AudioSource existing = pool[i];
				if (existing != null && existing.transform.parent != poolRoot.transform)
				{
					existing.transform.SetParent(poolRoot.transform);
					continue;
				}
				
				// No existing source in this slot, create one
				// We start the object as inactive, we'll activate it when we need it.
				var go = new GameObject($"{poolName} Pool Object [{i}]");
				go.SetActive(true);
				go.transform.SetParent(poolRoot.transform);
				existing = go.AddComponent<AudioSource>();
				existing.enabled = false;
				pool[i] = existing;
			}
		}

		public bool TryGetAudioSource(out AudioSource? pooledSource)
		{
			pooledSource = null!;
            
			AudioSource? attempt = this.GetNextAvailableAudioSource();
			if (attempt == null)
			{
				Debug.LogWarning($"[{poolName}] Pool's closed due to maximum audio source limit.");
				return false;
			}

			pooledSource = attempt;
			return true;
		}
		
		private void ReclaimUnusedAudioSources()
		{
			int lastStillPlaying = -1;
			for (var i = 0; i <= highestAcquiredIndex; i++)
			{
				AudioSource source = pool[i];
				if (!source.enabled)
					continue;

				if (!source.isPlaying)
				{
					source.enabled = false;
					if (highestAcquiredIndex <= i)
					{
						highestAcquiredIndex = lastStillPlaying;
					}
				}
				else
				{
					lastStillPlaying = i;
				}
			}
		}
		
		public AudioSource? GetNextAvailableAudioSource()
		{
			for (var i = 0; i < pool.Length; i++)
			{
				AudioSource source = pool[i];
				if (!source.enabled)
				{
					source.enabled = true;
					if (i >= highestAcquiredIndex)
						highestAcquiredIndex = i;
					return source;
				}
			}
			
			return null;
		}

		private sealed class AudioPoolHelper : MonoBehaviour
		{
			public AudioSourcePool? Pool { get; set; }
			
			private void Update()
			{
				Pool?.ReclaimUnusedAudioSources();
			}
		}
	}
}