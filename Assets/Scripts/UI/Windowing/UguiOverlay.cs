#nullable enable
using Core.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityExtensions;
using System.Collections.Generic;

namespace UI.Windowing
{
	public sealed class UguiOverlay : UIBehaviour
	{
		private static readonly Counter overlayCounter = new Counter();
		private static readonly List<UguiOverlay> overlays = new List<UguiOverlay>();
		
		private RectTransform rectTransform;
		private CanvasGroup canvasGroup;

		public RectTransform RectTransform => rectTransform;

		private int order;
		
		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();
			this.AssertAllFieldsAreSerialized(typeof(UguiOverlay));
			this.MustGetComponent(out canvasGroup);
			this.MustGetComponent(out rectTransform);

			canvasGroup.alpha = 0;
			
			overlays.Add(this);
			order = overlayCounter.Value;
			overlayCounter.CountUp();
		}

		/// <inheritdoc />
		protected override void OnEnable()
		{
			Vector3 anchored = this.rectTransform.anchoredPosition3D;
			anchored.z = order;
			this.rectTransform.anchoredPosition3D = anchored;
			
			base.OnEnable();

			LeanTween.alphaCanvas(canvasGroup, 1, 0.1f);
		}

		public void Close()
		{
			overlayCounter.CountDown();
			overlays.Remove(this);

			FixOverlays(order);
			
			LeanTween.alphaCanvas(canvasGroup, 0, 0.1f)
				.destroyOnComplete = true;
		}

		private static void FixOverlays(int o)
		{
			foreach (UguiOverlay overlay in overlays)
			{
				if (overlay.order < o)
					continue;

				overlay.order = o;

				Vector3 anchored = overlay.rectTransform.anchoredPosition3D;
				anchored.z = o;
				overlay.rectTransform.anchoredPosition3D = anchored;
			}
		}
	}
}