#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using Utility;

namespace UI.Backdrop
{
	public class BackdropController : MonoBehaviour
	{
		private Queue<BackdropSettings> pendingBackdropChanges = new Queue<BackdropSettings>();
		private Coroutine? backdropCoroutine;

		[Header("Components")]
		[SerializeField]
		private RawImage backdropImage = null!;

		[SerializeField]
		private RawImage temporaryBackdropImage = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(BackdropController));
			
			// Hide the temporary backdrop until it's time to change the backdrop.
			this.temporaryBackdropImage.gameObject.SetActive(false);
			
			// Initialize the backdrop to its default settings.
			SetBackdropSettingsInternal(backdropImage, BackdropSettings.Default);
		}

		private void SetBackdropSettingsInternal(RawImage target, BackdropSettings settings)
		{
			target.texture = settings.Texture;
			target.color = settings.ColorTint;
		}

		public void SetBackdrop(BackdropSettings settings)
		{
			pendingBackdropChanges.Enqueue(settings);

			if (backdropCoroutine == null)
				backdropCoroutine = StartCoroutine(ChangeBackdrop());
		}

		private IEnumerator ChangeBackdrop()
		{
			while (pendingBackdropChanges.Any())
			{
				// Get next backdrop settings object
				BackdropSettings nextBackdropSettings = pendingBackdropChanges.Dequeue();
				
				// Swap the backdrop images.
				backdropImage.transform.SetAsLastSibling();
				(backdropImage, temporaryBackdropImage) = (temporaryBackdropImage, backdropImage);
				
				// Set the new backdrop.
				SetBackdropSettingsInternal(backdropImage, nextBackdropSettings);
				
				// Enable the new backdrop
				backdropImage.gameObject.SetActive(true);
				
				// Start fading out the old one.
				float alpha = temporaryBackdropImage.color.a;
				float timeInSeconds = 0.5f;
				float timer = 0;

				while (timer < timeInSeconds)
				{
					yield return null;

					float newAlpha = alpha * Mathf.Clamp(1 - (timer / timeInSeconds), 0, 1);
					Color currentColor = temporaryBackdropImage.color;
					currentColor.a = newAlpha;
					temporaryBackdropImage.color = currentColor;

					timer += Time.deltaTime;
				}
				
				// Deactivate the temporary backdrop
				temporaryBackdropImage.gameObject.SetActive(false);
			}

			backdropCoroutine = null;
		}
	}
}