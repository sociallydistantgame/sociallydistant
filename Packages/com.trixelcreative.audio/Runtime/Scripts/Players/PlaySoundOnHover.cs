using System;
using System.Linq;
using TrixelCreative.TrixelAudio.Core;
using TrixelCreative.TrixelAudio.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrixelCreative.TrixelAudio.Players
{
	[Obsolete]
	public class PlaySoundOnHover :
		AudioPlayerBase,
		IPointerEnterHandler
	{
		[SerializeField]
		private SoundEffectAsset? onHoverSound;

		/// <inheritdoc />
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (onHoverSound == null)
				return;

			AudioManager.PlaySound(onHoverSound);
		}
	}

	public class PlaySoundOnImpact2D
		: AudioPlayerBase
	{
		private Collider2D[] colliders = Array.Empty<Collider2D>();

		[SerializeField]
		private SoundBankAsset soundBank = null!;

		[SerializeField]
		private string[] ignoredTags = Array.Empty<string>();

		[SerializeField]
		private LayerMask allowedLayers;
		
		/// <summary>
		///		Gets a value representing the total mass the object.
		/// </summary>
		public float TotalMass
		{
			get
			{
				Rect rect = Rect.zero;
				float averageDensity = 0;

				foreach (Collider2D col in this.colliders)
				{
					Vector3 center = col.bounds.center;
					Vector3 extents = col.bounds.extents;

					Vector2 topLeft = center - extents;
					Vector2 size = extents * 2;

					rect.x = Math.Min(rect.x, topLeft.x);
					rect.y = Math.Min(rect.y, topLeft.y);
					rect.width = Math.Max(rect.width, size.x);
					rect.height = Math.Max(rect.height, size.y);

					averageDensity += col.density;
				}

				averageDensity /= this.colliders.Length;

				return averageDensity * (rect.width * rect.height);
			}
		}
		
		/// <inheritdoc />
		protected override void OnAwake()
		{
			base.OnAwake();
			this.RefreshColliderList();
		}

		/// <summary>
		///		Refreshes the internal cache of <see cref="Collider2D"/> objects found on
		///		the object and its children. Call this after adding new colliders or child objects
		///		at runtime.
		/// </summary>
		public void RefreshColliderList()
		{
			this.colliders = this.GetComponentsInChildren<Collider2D>();
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			// Ignore any colliders that belong to us.
			if (this.colliders.Contains(col.collider))
				return;

			// Ignore objects in the tag blacklist
			if (ignoredTags.Contains(col.gameObject.tag))
				return;
			
			// Only interact with objects in the allowed layers
			if ((allowedLayers & (1 << col.gameObject.layer)) == 0)
				return;
			
			float ourMass = this.TotalMass;
			float colliderMass = col.collider.density * col.collider.bounds.size.x * col.collider.bounds.size.y;

			// Bigger object wins.
			if (ourMass >= colliderMass)
			{
				if (soundBank.TryGetRandomSound(out SoundEffectAsset sound))
					AudioManager.PlaySound(sound);
			}
		}
	}
}