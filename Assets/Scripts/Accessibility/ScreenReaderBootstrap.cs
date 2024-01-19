#nullable enable

using System;
using UnityEngine;
using UnityExtensions;

namespace Accessibility
{
	public class ScreenReaderBootstrap : MonoBehaviour
	{
		[SerializeField]
		private ScreenReaderHolder holder = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(ScreenReaderBootstrap));
			
			// If we're on Windows, we use the SAPI backend.
			#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
			{
				this.holder.Value = new Sapi5ScreenReader();
				return;
			}
			#endif
		}

		private async void Start()
		{
			if (holder.Value == null)
				return;

			await holder.Value.Speak("Screen reader service initialized. Performing self-test. Ian is cute.");
		}

		private void OnDestroy()
		{
			if (holder.Value == null)
				return;

			if (holder.Value is IDisposable disposable)
				disposable.Dispose();
			
			holder.Value = null;
		}
	}
}