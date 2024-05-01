#nullable enable

using System;
using TMPro;
using UnityEngine;
using UnityExtensions;
using Utility;

namespace UI.Popovers
{
	public class PopoverLayer : MonoBehaviour
	{
		private RectTransform myRect = null!;
		
		[Header("UI")]
		[SerializeField]
		private RectTransform popoverTransform = null!;
		
		[SerializeField]
		private CanvasGroup popoverCanvasGroup = null!;

		[SerializeField]
		private TextMeshProUGUI popoverLabel = null!;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(PopoverLayer));
			this.MustGetComponent(out myRect);

			HidePopover();
		}

		public void HidePopover()
		{
			popoverCanvasGroup.alpha = 0;
		}

		public void ShowPopover(RectTransform target, string text)
		{
			popoverLabel.SetText(text);

			// Calculate world-space rect of the target
			// and the popover label itself
			Vector2 targetSize = target.rect.size * target.lossyScale;
			Vector2 labelSize = popoverTransform.rect.size * popoverTransform.lossyScale;
			var targetWorldRect = new Rect((Vector2) target.position - (targetSize * target.pivot), targetSize);
			var labelRect = new Rect((Vector2) popoverTransform.position - (labelSize * popoverTransform.pivot), labelSize);
			
			// Determine the middle-right edge of the target
			Vector2 popoverTargetLocation = targetWorldRect.center + new Vector2(targetSize.x / 2, 0);
			
			// If the popover will end up being cut off of the screen, then we force it to the left.
			if (popoverTargetLocation.x + labelSize.x >= Screen.width)
				popoverTargetLocation.x -= (targetSize.x + labelSize.x);
			
			// Ensure we're lining the centre of the label horizontally with the target
			popoverTargetLocation.y -= (labelSize.y / 2);
			
			// Apply label pivot to the target position
			popoverTargetLocation += (labelSize * popoverTransform.pivot);

			// Move the popover
			popoverTransform.position = popoverTargetLocation;
			
			// Show it!
			popoverCanvasGroup.alpha = 1;
		}
	}
}